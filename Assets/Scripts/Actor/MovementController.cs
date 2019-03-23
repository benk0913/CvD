using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementController : MonoBehaviour {

    #region Client Control

    [SerializeField] bool DEBUG_TARGET = false;
    [SerializeField] Animator Animer;
    [SerializeField] Rigidbody2D Rigid;
    [SerializeField] Canvas StatsCanvas;
    [SerializeField] float Speed = 1f;
    [SerializeField] float JumpPower = 1f;
    [SerializeField] float MaxVelocity = 10f;

    [SerializeField]
    float GroundedThreshold;

    [SerializeField]
    LayerMask GroundLayerMask;

    [SerializeField]
    CapsuleCollider2D CurrentCollider;

    public CharacterInfo Character;

    public ActorState Status;

    public bool isPlayer;

    float lastXDir;
    float lastYDir;

    RaycastHit2D GroundRayRight;
    RaycastHit2D GroundRayLeft;


    Vector3 lastSentPos;

    public bool isGrounded
    {
        get
        {
            GroundRayRight = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(CurrentCollider.size.x / 16f, -CurrentCollider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);
            GroundRayLeft = Physics2D.Raycast(transform.position + transform.transform.TransformDirection(-CurrentCollider.size.x / 16f, -CurrentCollider.size.y / 13f, 0), -transform.up, GroundedThreshold, GroundLayerMask);

            return (GroundRayRight || GroundRayLeft);
        }
    }

    public bool isFalling
    {
        get
        {
            return Rigid.velocity.y < 0f;
        }
    }


    private void Update()
    {
        if (isPlayer)
        {
            RefreshInput();
        }
    }

    void RefreshInput()
    {
        if(Status.MovementAbilityRoutineInstance != null)
        {
            return;
        }

        if (Input.GetKey(InputMap.Map["Walk Left"]))
        {
            WalkLeft();

        }
        else if (Input.GetKey(InputMap.Map["Walk Right"]))
        {
            WalkRight();

        }
        else
        {
            StandStill();

        }

        if (Input.GetKeyDown(InputMap.Map["Space Ability"]) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(InputMap.Map["Shift Ability"]))
        {
            Hurt(1);
        }

        Animer.SetBool("inAir", !isGrounded && isFalling);

        for(int i=0;i< Character.Class.Abilities.Count;i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Ability"+(i+1)]))
            {
                StartAbility(Character.Class.Abilities[i]);
            }
        }
    }

    void WalkLeft()
    {
        Animer.SetBool("Run", true);
        Animer.transform.localScale = new Vector3(1f, 1f, 1f);

        Rigid.position += (Vector2)transform.right * -Speed * Time.deltaTime;

        lastXDir = -Speed;
    }

    void WalkRight()
    {
        Animer.SetBool("Run", true);
        Animer.transform.localScale = new Vector3(-1f, 1f, 1f);

        Rigid.position += (Vector2)transform.right * Speed * Time.deltaTime;

        lastXDir = Speed;
    }

    void StandStill()
    {
        Animer.SetBool("Run", false);

        lastXDir = 0f;
    }
    
    void Jump()
    {
        if(JumpRoutineInstance != null)
        {
            StopCoroutine(JumpRoutineInstance);
        }

        JumpRoutineInstance = StartCoroutine(JumpRoutine());
    }

    Coroutine JumpRoutineInstance;
    IEnumerator JumpRoutine()
    {
        yield return 0;
        Rigid.AddForce(transform.up * JumpPower, ForceMode2D.Impulse);
        Animer.SetTrigger("Jump");

        JumpRoutineInstance = null;
    }

    private void FixedUpdate()
    {
        if (isPlayer && (lastSentPos != transform.position))
        {
            SocketClient.Instance.EmitMovement(transform.position, lastXDir, Rigid.velocity.y);
            lastSentPos = transform.position;
        }

        if (Rigid.velocity.magnitude > MaxVelocity)
        {
            Rigid.velocity = Rigid.velocity.normalized * MaxVelocity;
        }
    }

    #region Abilities

    public void StartAbility(Ability ability)
    {
        if (isPlayer)
        {
            if (AbilityDurationRoutineInstance != null)
            {
                //TODO - CURRENTLY DOING AN ABILITY ALERT!
                return;
            }

            AbilityStatus abilityStatus = Status.GetAbilityStatus(ability);
            if (abilityStatus != null && abilityStatus.CooldownRoutine != null)
            {
                //TODO - IN COOLDOWN ALERT!
                return;
            }

            AnimateStartAbility(ability);

            StartAbilityDuration(ability);
        }
        else
        {
            AnimateStartAbility(ability);
        }
    }

    void StartAbilityDuration(Ability ability)
    {
        if(AbilityDurationRoutineInstance != null)
        {
            return;
        }

        AbilityDurationRoutineInstance = StartCoroutine(AbilityDurationRoutine(ability));
    }

    Coroutine AbilityDurationRoutineInstance;
    IEnumerator AbilityDurationRoutine(Ability ability)
    {
        yield return new WaitForSeconds(ability.Duration);

        CompleteAbility(ability);
        AbilityDurationRoutineInstance = null;
    }

    public void CompleteAbility(Ability ability)
    {
        SpawnAbilityObjects(ability);
        ActivateAbilityPerks(ability);

        SocketClient.Instance.SendUsedAbility(ability.name);


        InGamePanelUI.Instance.ActivateAbility(ability);

        StartCooldown(ability);
    }


    private void StartCooldown(Ability ability)
    {
        AbilityStatus abilityStatus = Status.GetAbilityStatus(ability);

        if(abilityStatus == null)
        {
            return;
        }

        if (abilityStatus.CooldownRoutine != null)
        {
            StopCoroutine(abilityStatus.CooldownRoutine);
        }

        abilityStatus.CooldownRoutine = StartCoroutine(CooldownRoutine(abilityStatus));
    }

    IEnumerator CooldownRoutine(AbilityStatus abilityStatus)
    {
        yield return new WaitForSeconds(abilityStatus.Reference.Cooldown);

        abilityStatus.CooldownRoutine = null;
    }


    public void SpawnAbilityObjects(Ability ability)
    {
        GameObject tempObj;
        for (int i = 0; i < ability.ObjectsToSpawn.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject(ability.ObjectsToSpawn[i]);

            HitBoxScript hitbox = tempObj.GetComponent<HitBoxScript>();
            
            if (hitbox != null)
            {
                HitboxEvent HitEvent = new HitboxEvent();
                HitEvent.AddListener(OnHitboxEvent);

                tempObj.GetComponent<HitBoxScript>().SetInfo(this.Character, ability, HitEvent);
            }

            tempObj.transform.position = transform.position;
            tempObj.transform.localScale =
                new Vector3(
                    Mathf.Abs(tempObj.transform.localScale.x) * ((Animer.transform.localScale.x < 0) ? -1f : 1f),
                    tempObj.transform.localScale.y,
                    tempObj.transform.localScale.z);
        }
    }

    private void OnHitboxEvent(Ability ability)
    {
        StartAbility(ability.AbilityOnHit);
    }

    public void ActivateAbilityPerks(Ability ability)
    {
        for (int i = 0; i < ability.Perks.Count; i++)
        {
            ActivatePerk(ability.Perks[i]);
        }
    }

    public void ActivatePerk(Perk perk)
    {
        switch (perk.Attribute.name)
        {
            case "Movement":
                {
                    ActivateMovementPerk(perk);
                    break;
                }
            default:
                {
                    return;
                }
        }
    }

    public void ActivateMovementPerk(Perk perk)
    {
        if (Status.MovementAbilityRoutineInstance != null)
        {
            StopCoroutine(Status.MovementAbilityRoutineInstance);
        }

        Status.MovementAbilityRoutineInstance
                        = StartCoroutine(perk.name + "AbilityRoutine", perk);
    }

    public void ShutMovementAbility()
    {
        Animer.Play("InAir");
        Status.MovementAbilityRoutineInstance = null;
    }

    #region Movement Abilities Implementation

    //Yes, hardcoded, for now.
    IEnumerator InterruptMovementAbilityRoutine(Perk perk)
    {
        yield return 0;
        ShutMovementAbility();
    }

    IEnumerator PounceAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed    = perk.GetPerkValueByType("SpeedModifier", 3f);
        bool isInterruptOnGrounded = (perk.GetPerkValue("interruptOnGrounded", 0f) > 0f);

        Vector2 direction = Vector2.down + (Animer.transform.localScale.x > 0 ? Vector2.left : Vector2.right);

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * Time.deltaTime;

            if(isInterruptOnGrounded && isGrounded)
            {
                ShutMovementAbility();
                yield break;
            }

            yield return 0;
        }

        yield return 0;

        Status.MovementAbilityRoutineInstance = null;
    }

    IEnumerator RicochetAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 1f);
        bool isInterruptOnGrounded = (perk.GetPerkValue("interruptOnGrounded", 0f) > 0f);

        Vector2 direction = (Vector2.up * 1.5f) + (Animer.transform.localScale.x > 0 ? Vector2.right : Vector2.left);

        float initDuration = duration;

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * (duration / initDuration) * Time.deltaTime;

            if(duration > 0.5f && isInterruptOnGrounded && isGrounded)
            {
                ShutMovementAbility();
                yield break;
            }

            yield return 0;
        }

        Status.MovementAbilityRoutineInstance = null;
    }

    #endregion

    #endregion

    #endregion

    #region Shared

    private void OnEnable()
    {
        if (DEBUG_TARGET)
        {
            this.Character = new CharacterInfo("123", "Debug Target", CORE.Instance.Data.GetClass("Test_Class"));
            Status.Initialize(this.Character);
        }
    }

    public void SetInfo(CharacterInfo info, bool isplayer)
    {
        this.isPlayer = isplayer;
        this.Character = info;

        Status.Initialize(this.Character);
    }

    public void AnimateStartAbility(Ability ability)
    {
        if(ability.Animations.Count == 0)
        {
            return;
        }

        Animer.Play(ability.Animations[UnityEngine.Random.Range(0, ability.Animations.Count)]);
    }

    public void Hurt(int damage)
    {
        ShowDamageText(damage);

        Animer.Play(Character.Class.HurtAnimations[UnityEngine.Random.Range(0, Character.Class.HurtAnimations.Count)]);

        Status.CurrentHP -= damage;
    }

    public void ShowDamageText(int damage)
    {
        string prefabName;

        if(isPlayer)
        {
            prefabName = "FloatingDamage_Self";
        }
        else
        {
            prefabName = "FloatingDamage_Target";
        }

        GameObject tempDamageText = ResourcesLoader.Instance.GetRecycledObject(CORE.Instance.PrefabDatabase[prefabName]);

        tempDamageText.transform.position = StatsCanvas.transform.position;
        tempDamageText.GetComponent<FloatingDamageUI>().Activate(damage.ToString());
    }

    #endregion

    #region Server Control

    Vector3 LastGivenPosition;
    Coroutine FixPositionRoutineInstance;
    float lastDirectionX;
    float lastDirectionY;

    float TimeFromLastEvent;

    [SerializeField]
    float ExtrapolationMultiplier = 0.05f;

    [SerializeField]
    float DeltaMultiplier = 2f;

    bool lastIsGrounded;

    public void SetLastPosition(Vector2 position, float dirX, float dirY)
    {
        lastDirectionY = dirY;

        lastDirectionX = dirX;
        LastGivenPosition = new Vector3(position.x + dirX * (ExtrapolationMultiplier * TimeFromLastEvent), position.y);

        if (FixPositionRoutineInstance != null)
        {
            StopCoroutine(FixPositionRoutineInstance);
        }
        FixPositionRoutineInstance = StartCoroutine(FixPositionRoutine());

        if (dirX > 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            Animer.SetBool("Run", true);
        }
        else if (dirX < 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            Animer.SetBool("Run", true);
        }
        else
        {
            Animer.SetBool("Run", false);
        }

        TimeFromLastEvent = 1f;

        if (!isGrounded && lastIsGrounded)
        {
            Animer.SetTrigger("Jump");
        }

        lastIsGrounded = isGrounded;
    }

    private void LateUpdate()
    {
        Animer.SetBool("inAir", (!isGrounded && isFalling));

        TimeFromLastEvent += DeltaMultiplier * Time.deltaTime;
    }

    private IEnumerator FixPositionRoutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += 3f * Time.deltaTime;

            Rigid.position = Vector3.Lerp(Rigid.position, LastGivenPosition, t);

            if (lastDirectionY > 0f)
            {
                Rigid.gravityScale = 0f;
            }
            else
            {
                Rigid.gravityScale = 1f;
            }

            yield return 0;
        }

        FixPositionRoutineInstance = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBoxScript hitbox = collision.GetComponent<HitBoxScript>();

        if (DEBUG_TARGET) //TODO - REMOVE LATER / DEBUG PURPOSES....
        {
            Hurt(UnityEngine.Random.Range(1, 4));
            SpawnAbilityObjects(hitbox.CurrentAbility.AbilityOnHit);

            hitbox.Interact("test");
            return;
        }

        if (!isPlayer && collision.tag == "HitBox")
        {
            CharacterInfo chara = CORE.Instance.CurrentRoom.GetPlayer(this.gameObject);

            hitbox.Interact(chara.Name);
            
        }
    }

    #endregion
}

[System.Serializable]
public class HitboxEvent : UnityEvent<Ability>
{
}
