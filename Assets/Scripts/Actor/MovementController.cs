using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

    #region Client Control

    [SerializeField] Animator Animer;
    [SerializeField] Rigidbody2D Rigid;
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
            Hurt();
        }

        Animer.SetBool("inAir", !isGrounded && isFalling);

        for(int i=0;i< Character.Class.Abilities.Count;i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Ability"+(i+1)]))
            {
                ActivateAbility(Character.Class.Abilities[i]);
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
        if (isPlayer)
        {
            SocketClient.Instance.EmitMovement(transform.position, lastXDir, Rigid.velocity.y);
            lastSentPos = transform.position;
        }

        if (Rigid.velocity.magnitude > MaxVelocity)
        {
            Rigid.velocity = Rigid.velocity.normalized * MaxVelocity;
        }
    }

    #endregion

    #region Shared

    public void SetInfo(CharacterInfo info, bool isplayer)
    {
        this.isPlayer = isplayer;
        this.Character = info;
    }

    public void ActivateAbility(Ability ability)
    {
        Animer.Play(ability.Animations[UnityEngine.Random.Range(0, ability.Animations.Count)]);

        if (isPlayer)
        {
            SocketClient.Instance.SendUsedAbility(ability.name);

            GameObject tempObj;
            for (int i = 0; i < ability.ObjectsToSpawn.Count; i++)
            {
                tempObj = ResourcesLoader.Instance.GetRecycledObject(ability.ObjectsToSpawn[i]);
                tempObj.GetComponent<HitBoxScript>().SetInfo(this.Character.ID, ability);
            }
        }

        //Animer.SetInteger("AbilityID", ability.AnimationsIDs[UnityEngine.Random.Range(0, ability.AnimationsIDs.Count)]);
        //Animer.SetTrigger("Ability");

        //SocketClient.Instance.SendPreformedAttack(1f, 0);
        //GameObject obj = ResourcesLoader.Instance.GetRecycledObject("HitBox");
        //obj.transform.position = transform.position;
        //obj.GetComponent<HitBoxScript>().SetInfo();
    }

    public void Hurt()
    {
        Animer.Play(Character.Class.HurtAnimations[UnityEngine.Random.Range(0, Character.Class.HurtAnimations.Count)]);
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
        if(!isPlayer && collision.tag == "HitBox")
        {
            CharacterInfo chara = CORE.Instance.CurrentRoom.GetPlayer(this.gameObject);

            collision.GetComponent<HitBoxScript>().Interact(chara.Name);
        }
    }

    #endregion
}
