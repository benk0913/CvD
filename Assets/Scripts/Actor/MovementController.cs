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
    [SerializeField] SpriteAlphaGroup AlphaGroup;
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
    public bool isDead;
    public bool isStunned = false;
    public bool isCastingAbility = false;
    public bool isRecentlyHurt = false;

    //Should be used only by "Movement Abilities"
    float movementAbilitySpeedModifier = 1f;

    //Use only this for speed
    public float FinalSpeed
    {
        get
        {
            float speedResult = this.Speed;

            if(isCastingAbility)
            {
                speedResult -= this.Speed * 0.3f;
            }

            if (isRecentlyHurt)
            {
                speedResult -= this.Speed * 0.2f;
            }

            foreach (BuffStatus buff in Status.ActiveBuffs)
            {
                for (int i = 0; i < buff.Reference.Perks.Count; i++)
                {
                    if (buff.Reference.Perks[i].Attribute.name == "SpeedModifier")
                    {
                        speedResult += (this.Speed * buff.Reference.Perks[i].MinValue);
                        break;
                    }
                }
            }

            speedResult *= movementAbilitySpeedModifier;

            
            return Mathf.Clamp(speedResult, 0.1f, Mathf.Infinity);
        }
    }

    
    float lastXDir;
    float lastYDir;

    RaycastHit2D GroundRayRight;
    RaycastHit2D GroundRayLeft;
    
    Vector3 lastSentPos;

    CharacterInfo lastOffender;


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

    public bool isDuringAbility
    {
        get
        {
            if (Status.MovementAbilityRoutineInstance != null)
            {
                return true;
            }

            if(AbilityDurationRoutineInstance != null)
            {
                return true;
            }

            return false;
        }
    }

    public bool isFacingLeft
    {
        get { return Animer.transform.localScale.x > 0;  }
    }

    public bool isGoingUp
    {
        get { return Rigid.velocity.y >= 0; }
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
        
        if(isDead)
        {
            return;
        }

        if(isStunned)
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
            Hurt(1); // TODO Replace this with w/e!
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

        Rigid.position += (Vector2)transform.right * -FinalSpeed * Time.deltaTime;

        lastXDir = -FinalSpeed;
    }

    void WalkRight()
    {
        Animer.SetBool("Run", true);
        Animer.transform.localScale = new Vector3(-1f, 1f, 1f);

        Rigid.position += (Vector2)transform.right * FinalSpeed * Time.deltaTime;

        lastXDir = FinalSpeed;
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
            if (abilityStatus != null && abilityStatus.Charges <= 0)
            {
                //TODO - IN COOLDOWN ALERT!
                return;
            }

            if(ability.AbilityOnLeft != null && isFacingLeft)
            {
                StartAbility(ability.AbilityOnLeft);
                return;
            }

            SocketClient.Instance.SendUsedAbility(ability.name);
        }

        AnimateStartAbility(ability);

        StartAbilityDuration(ability);
    }

    void StartAbilityDuration(Ability ability)
    {
        AbilityStatus abilityStatus = Status.GetAbilityStatus(ability);
        if(abilityStatus != null && abilityStatus.Charges <= 0)
        {
            return;
        }
        
        AbilityDurationRoutineInstance = StartCoroutine(AbilityDurationRoutine(ability));
        
    }

    Coroutine AbilityDurationRoutineInstance;
    IEnumerator AbilityDurationRoutine(Ability ability)
    {
        float duration = ability.Duration;

        if(!isPlayer) // Lag compensation
        {
            duration = Mathf.Clamp(duration - TimeFromLastEvent ,0f ,Mathf.Infinity);
        }

        isCastingAbility = true;

        yield return new WaitForSeconds(duration);

        isCastingAbility = false;

        CompleteAbilityDuration(ability);
        AbilityDurationRoutineInstance = null;
    }

    public void CompleteAbilityDuration(Ability ability)
    {
        SpawnAbilityObjects(ability);

        if (isPlayer)
        {
            ActivateAbilityPerks(ability);
            
            StartCooldown(ability);
        }
    }


    private void StartCooldown(Ability ability)
    {
        AbilityStatus abilityStatus = Status.GetAbilityStatus(ability);

        if(abilityStatus == null)
        {
            return;
        }

        if (abilityStatus.Charges <= 0)
        {
            return;
        }

        abilityStatus.UpdateCooldown(abilityStatus.Reference.Cooldown);
        
        abilityStatus.ActivateAbility(StartCoroutine(CooldownRoutine(abilityStatus)));
    }

    IEnumerator CooldownRoutine(AbilityStatus abilityStatus)
    {
        while(abilityStatus.CooldownSecondsLeft > 0)
        {
            if (abilityStatus.Reference.HasTimerCountdown)
            {
                abilityStatus.CooldownSecondsLeft -= 1f * Time.deltaTime;

                abilityStatus.UpdateCooldown(abilityStatus.CooldownSecondsLeft);
            }

            yield return 0;
        }

        abilityStatus.CompleteCooldown();

        if(abilityStatus.Charges < abilityStatus.Reference.ChargesCap)
        {
            abilityStatus.StartRechargeCooldown(StartCoroutine(CooldownRoutine(abilityStatus)));
        }
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

                tempObj.GetComponent<HitBoxScript>().SetInfo(this.Character, ability, HitEvent, isPlayer);
            }

            tempObj.transform.position = transform.position;
            tempObj.transform.localScale =
                new Vector3(
                    Mathf.Abs(tempObj.transform.localScale.x) * ((Animer.transform.localScale.x < 0) ? -1f : 1f),
                    tempObj.transform.localScale.y,
                    tempObj.transform.localScale.z);
        }
    }

    public void SpawnTargetAbilityObjects(Ability ability)
    {
        GameObject tempObj;
        for (int i = 0; i < ability.ObjectsToSpawnOnTargets.Count; i++)
        {
            tempObj = ResourcesLoader.Instance.GetRecycledObject(ability.ObjectsToSpawnOnTargets[i]);

            HitBoxScript hitbox = tempObj.GetComponent<HitBoxScript>();

            if (hitbox != null)
            {
                HitboxEvent HitEvent = new HitboxEvent();
                HitEvent.AddListener(OnHitboxEvent);

                tempObj.GetComponent<HitBoxScript>().SetInfo(this.Character, ability, HitEvent, isPlayer);
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
        if(ability.AbilityOnHit == null)
        {
            return;
        }

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

    public void ApplyMovementBuff(BuffStatus buffStatus, CharacterInfo fromPlayer)
    {
        if (this.Character.isPlayer)
        {
            for (int i = 0; i < buffStatus.Reference.Perks.Count; i++)
            {
                if (buffStatus.Reference.Perks[i].Attribute.name == "Movement")
                {
                    lastOffender = fromPlayer;
                    ActivateMovementPerk(buffStatus.Reference.Perks[i]);
                }
            }
        }
    }

    public void ActivateMovementPerk(Perk perk)
    {

        if (Status.MovementAbilityRoutineInstance != null)
        {
            StopCoroutine(Status.MovementAbilityRoutineInstance);
        }

        string abilityRoutineName;

        if(!string.IsNullOrEmpty(perk.DisplayName))
        {
            abilityRoutineName = perk.DisplayName;
        }
        else
        {
            abilityRoutineName = perk.name;
        }

        Status.MovementAbilityRoutineInstance
                     = StartCoroutine(abilityRoutineName + "AbilityRoutine", perk);
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
        
        Vector2 direction = (isGoingUp? Vector2.up : Vector2.down) + (isFacingLeft ? Vector2.left : Vector2.right);

        if(isGoingUp)
        {
            Animer.Play("TestChar_PounceUp");
        }

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * Time.deltaTime;

            if(!isGoingUp && isInterruptOnGrounded && isGrounded)
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

        Vector2 direction = (Vector2.up * 1.5f) + (isFacingLeft ? Vector2.left : Vector2.right);

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

    IEnumerator PushbackAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 1f);

        Vector2 direction = -((Vector2)(lastOffender.CInstance.transform.position - transform.position)) + Vector2.up;

        float initDuration = duration;

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * (duration / initDuration) * Time.deltaTime;

            yield return 0;
        }

        Status.MovementAbilityRoutineInstance = null;
    }

    IEnumerator PullAbilityRoutine(Perk perk)
    {
        float speed = perk.GetPerkValueByType("SpeedModifier", 1f);

        Vector3 initPos = Rigid.position;
        Vector3 targetPos = lastOffender.CInstance.transform.position;
        float randomHeight = UnityEngine.Random.Range(1f, 5f);

        float duration = 0f;
        while (duration < 1f)
        {
            duration += speed * Time.deltaTime;

            Rigid.position = CORE.SplineLerp(initPos, targetPos, randomHeight, duration);

            yield return 0;
        }

        Status.MovementAbilityRoutineInstance = null;
    }


    IEnumerator ChargeAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 1f);

        float initDuration = duration;

        bool initIsFacingLeft = isFacingLeft;

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;

            movementAbilitySpeedModifier = 1.5f;

            if (isFacingLeft)
            {
                WalkLeft();
            }
            else
            {
                WalkRight();
            }

            yield return 0;
        }

        movementAbilitySpeedModifier = 1f;

        Status.MovementAbilityRoutineInstance = null;
    }

    IEnumerator StartHasteAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 1f);

        float initDuration = duration;

        Vector2 direction = isFacingLeft ? Vector2.left : Vector2.right;

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;

            if(duration > 0.5f)
            {
                Rigid.position += direction * speed * duration;
            }
            else
            {
                Rigid.position += direction * speed * (1f-duration);
            }

            yield return 0;
        }

        Status.MovementAbilityRoutineInstance = null;
    }


    IEnumerator AttackForwardAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 3f);

        Vector2 direction = (isFacingLeft ? Vector2.left : Vector2.right);

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * Time.deltaTime;

            yield return 0;
        }

        yield return 0;

        Status.MovementAbilityRoutineInstance = null;
    }

    IEnumerator AttackBackwardAbilityRoutine(Perk perk)
    {
        float duration = perk.GetPerkValueByType("DurationModifier", 1f);
        float speed = perk.GetPerkValueByType("SpeedModifier", 3f);

        Vector2 direction = (isFacingLeft ? Vector2.right : Vector2.left);

        while (duration > 0)
        {
            duration -= 1f * Time.deltaTime;
            Rigid.position += direction * speed * Time.deltaTime;

            yield return 0;
        }

        yield return 0;

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
        
        foreach(Ability ability in Character.Class.Abilities)
        {
            if(ability.StartWithCooldown)
            {
                StartCooldown(ability);
            }
        }
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

        if (!isDuringAbility)
        {
            Animer.Play(Character.Class.HurtAnimations[UnityEngine.Random.Range(0, Character.Class.HurtAnimations.Count)]);
        }

        Status.CurrentHP -= damage;

        if(HurtEffectRoutineInstance != null)
        {
            StopCoroutine(HurtEffectRoutineInstance);
        }

        HurtEffectRoutineInstance = StartCoroutine(HurtEffectRoutine());
        AlphaGroup.BlinkColor(Color.black);
    }

    public void Heal(int health)
    {
        ShowHealText(health);

        Status.CurrentHP += health;

        AlphaGroup.BlinkColor(Color.green);
    }

    public void Blocked()
    {
        ShowBlockedText();
        AlphaGroup.BlinkColor(Color.grey);

        Animer.Play("Block");
    }

    public void Death()
    {
        Animer.Play(Character.Class.DeathAnimations[UnityEngine.Random.Range(0, Character.Class.DeathAnimations.Count)]);
        isDead = true;

        if(DeathRoutineInstance != null)
        {
            return;
        }

        DeathRoutineInstance = StartCoroutine(DeathRoutine());
    }

    Coroutine DeathRoutineInstance;
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

        AlphaGroup.FadeOut(1f);

        yield return new WaitForSeconds(2f);

        if (this.Character.CInstance == this)
        {
            this.Character.CInstance = null;
        }

        Destroy(this.gameObject);
    }

    Coroutine HurtEffectRoutineInstance;
    IEnumerator HurtEffectRoutine()
    {
        isRecentlyHurt = true;

        yield return new WaitForSeconds(0.1f);

        isRecentlyHurt = false;
        HurtEffectRoutineInstance = null;
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

        tempDamageText.transform.position = StatsCanvas.transform.position
            + StatsCanvas.transform.TransformDirection(
                UnityEngine.Random.Range(-0.1f, 0.1f),
                UnityEngine.Random.Range(-0.1f, 0.1f),
                0f);

        tempDamageText.GetComponent<FloatingDamageUI>().Activate(damage.ToString());
    }

    public void ShowHealText(int health)
    {
        string prefabName;

        if (isPlayer)
        {
            prefabName = "FloatingHeal_Self";
        }
        else
        {
            prefabName = "FloatingHeal_Target";
        }

        GameObject tempDamageText = ResourcesLoader.Instance.GetRecycledObject(CORE.Instance.PrefabDatabase[prefabName]);

        tempDamageText.transform.position = StatsCanvas.transform.position 
            + StatsCanvas.transform.TransformDirection(
                UnityEngine.Random.Range(-0.1f, 0.1f),
                UnityEngine.Random.Range(-0.1f, 0.1f),
                0f);

        tempDamageText.GetComponent<FloatingDamageUI>().Activate(health.ToString());
    }

    public void ShowBlockedText()
    {
        string prefabName;

        if (isPlayer)
        {
            prefabName = "FloatingBlocked_Self";
        }
        else
        {
            prefabName = "FloatingBlocked_Target";
        }

        GameObject tempDamageText = ResourcesLoader.Instance.GetRecycledObject(CORE.Instance.PrefabDatabase[prefabName]);

        tempDamageText.transform.position = StatsCanvas.transform.position
            + StatsCanvas.transform.TransformDirection(
                UnityEngine.Random.Range(-0.1f, 0.1f),
                UnityEngine.Random.Range(-0.1f, 0.1f),
                0f);

        tempDamageText.GetComponent<FloatingDamageUI>().Activate("BLOCKED");
    }


    public void AddBuff(Buff buff, CharacterInfo fromPlayer)
    {
        GameObject buffPrefab = null;

        if(buff.BuffPrefab != null)
        {
            buffPrefab = ResourcesLoader.Instance.GetRecycledObject(buff.BuffPrefab);
            buffPrefab.transform.SetParent(transform);
            buffPrefab.transform.position = transform.position;
        }

        if (!string.IsNullOrEmpty(buff.AddBuffAnimation))
        {
            Animer.Play(buff.AddBuffAnimation);
        }

        BuffStatus buffStatus = new BuffStatus(buff, buffPrefab);

        Status.ActiveBuffs.Add(buffStatus);


        ActivateMovementBuff(buffStatus, fromPlayer);

    }

    public void RemoveBuff(Buff buff)
    {
        BuffStatus status = Status.GetBuffStatus(buff);
        status.OnClearEvent.Invoke();

        if(status.BuffPrefab != null)
        {
            status.BuffPrefab.transform.SetParent(null);
            status.BuffPrefab.SetActive(false);
        }

        if (!string.IsNullOrEmpty(buff.RemoveBuffAnimation))
        {
            Animer.Play(buff.RemoveBuffAnimation);
        }

        Status.ActiveBuffs.Remove(status);

        DeactivateMovementBuff(status);
    }

    void ActivateMovementBuff(BuffStatus buffStatus, CharacterInfo fromPlayer)
    {
        ApplyMovementBuff(buffStatus, fromPlayer);

        switch (buffStatus.Reference.name)
        {
            case "Stun":
                {
                    isStunned = true;
                    Animer.Play(Character.Class.StunnedAnimations[UnityEngine.Random.Range(0, Character.Class.StunnedAnimations.Count)]);

                    InterruptAbility();
                    return;
                }
        }
    }

    void DeactivateMovementBuff(BuffStatus buffStatus)
    {
        switch (buffStatus.Reference.name)
        {
            case "Stun":
                {
                    if (Status.GetBuffStatus(buffStatus.Reference) == null)
                    {
                        isStunned = false;
                    }

                    return;
                }
        }
    }

    void InterruptAbility()
    {
        if(AbilityDurationRoutineInstance != null)
        {
            StopCoroutine(AbilityDurationRoutineInstance);
            AbilityDurationRoutineInstance = null;
        }
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

        lastDirectionX = dirX;//  Lag Compensation
        LastGivenPosition = new Vector3(position.x + dirX * (ExtrapolationMultiplier * (TimeFromLastEvent < 1.5f? TimeFromLastEvent : 0f)), position.y);

        if (FixPositionRoutineInstance != null)
        {
            StopCoroutine(FixPositionRoutineInstance);
        }
        FixPositionRoutineInstance = StartCoroutine(FixPositionRoutine());

        if (dirX > 0f)
        {
            Animer.transform.localScale = new Vector3(-1f, 1f, 1f);
            Animer.SetBool("Run", true);
        }
        else if (dirX < 0f)
        {
            Animer.transform.localScale = new Vector3(1f, 1f, 1f);
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

            hitbox.Interact("test");
            SpawnTargetAbilityObjects(hitbox.CurrentAbility);
            return;
        }

        if (collision.tag == "HitBox" && hitbox.CurrentOwner.ID != Character.ID)
        {
            if (!isPlayer)
            {
                CharacterInfo chara = CORE.Instance.CurrentRoom.GetPlayer(this.gameObject);

                hitbox.Interact(chara.ID);
                SpawnTargetAbilityObjects(hitbox.CurrentAbility);
            }
            if (isPlayer)
            {
                hitbox.Interact(Character.ID);
                SpawnTargetAbilityObjects(hitbox.CurrentAbility);
            }
        }
    }

    #endregion
}

[System.Serializable]
public class HitboxEvent : UnityEvent<Ability>
{
}
