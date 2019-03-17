using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

    #region Client Control

    [SerializeField] Animator Animer;
    [SerializeField] Rigidbody2D Rigid;
    [SerializeField] float Speed = 1f;
    [SerializeField] float Jump = 1f;

    [SerializeField]
    float GroundedThreshold;

    [SerializeField]
    LayerMask GroundLayerMask;

    [SerializeField]
    CapsuleCollider2D CurrentCollider;

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
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Animer.SetBool("Run", true);
                Animer.transform.localScale = new Vector3(1f, 1f, 1f);

                Rigid.position += (Vector2)transform.right * -Speed * Time.deltaTime;

                lastXDir = -Speed;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Animer.SetBool("Run", true);
                Animer.transform.localScale = new Vector3(-1f, 1f, 1f);

                Rigid.position += (Vector2)transform.right * Speed * Time.deltaTime;

                lastXDir = Speed;
            }
            else
            {
                Animer.SetBool("Run", false);

                lastXDir = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Rigid.AddForce(transform.up * Jump, ForceMode2D.Impulse);
                Animer.SetTrigger("Jump");
            }

            Animer.SetBool("inAir", !isGrounded && isFalling);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Animer.SetInteger("AttackID", UnityEngine.Random.Range(0, 3));
                Animer.SetTrigger("Attack");
                SocketClient.Instance.SendPreformedAttack(1f, 0);

                GameObject obj = ResourcesLoader.Instance.GetRecycledObject("HitBox");
                obj.transform.position = transform.position;
                obj.GetComponent<HitBoxScript>().SetInfo();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Animer.SetTrigger("Hurt");
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPlayer)
        {
            //if (lastSentPos != transform.position)
            //{
                SocketClient.Instance.EmitMovement(transform.position, lastXDir, Rigid.velocity.y);
                lastSentPos = transform.position;
            //}
        }
    }

    #endregion

    public void Hurt()
    {
        Animer.SetTrigger("Hurt");
    }

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
    }

    private void LateUpdate()
    {
        if (!isGrounded && lastIsGrounded)
        {
            Animer.SetTrigger("Jump");
        }

        lastIsGrounded = isGrounded;

        Animer.SetBool("inAir", (!isGrounded && isFalling));

        TimeFromLastEvent += DeltaMultiplier * Time.deltaTime;
    }

    private IEnumerator FixPositionRoutine()
    {
        float t = 0f;
        while(t<1f)
        {
            t += 3f * Time.deltaTime;

            Rigid.position = Vector3.Lerp(Rigid.position, LastGivenPosition, t);

            if(lastDirectionY > 0f)
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

    public void PreformAttack()
    {
        Animer.SetTrigger("Attack");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isPlayer && collision.tag == "HitBox")
        {
            List<string> ids = new List<string>();
            CharacterInfo chara = CORE.Instance.CurrentRoom.GetPlayer(this.gameObject);


            ids.Add(chara.Name);
            SocketClient.Instance.SendUsedPrimaryAbility(ids, 0);

            collision.GetComponent<HitBoxScript>().Interact();
        }
    }

    #endregion
}
