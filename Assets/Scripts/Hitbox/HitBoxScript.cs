using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBoxScript : MonoBehaviour {

    [SerializeField]
    protected float timeLeft = 0.1f;

    protected float timeLeftCurrent;

    [SerializeField]
    protected bool ShutOnHit = true;

    [SerializeField]
    protected bool StickToOwner = false;

    [SerializeField]
    protected bool DependsOnMovementAbility = false;

    [SerializeField][Tooltip("Continious Damage will send 'Hit Events' per player entering and not by the regular way (per 'Hitbox Destruction') ")]
    protected bool IsContiniousDamage = false;

    [SerializeField]
    Collider2D Collider;

    [System.NonSerialized]
    public Ability CurrentAbility;

    [System.NonSerialized]
    public List<string> ActorsHit = new List<string>();

    [System.NonSerialized]
    public CharacterInfo CurrentOwner;

    [System.NonSerialized]
    protected MovementController CurrentOwnerMovementController;



    protected HitboxEvent CurrentHitEvent;

    protected bool isPlayer;

    protected bool WasHitThisFrame;

    public virtual void SetInfo(CharacterInfo ownerInsance,Ability ability, HitboxEvent onHitEvent, bool isplayer)
    {
        this.isPlayer = isplayer;

        ActorsHit.Clear();
        WasHitThisFrame = false;

        this.CurrentAbility = ability;
        this.CurrentOwner = ownerInsance;
        this.CurrentOwnerMovementController = CurrentOwner.CInstance.GetComponent<MovementController>();
        this.CurrentHitEvent = onHitEvent;

        timeLeftCurrent = timeLeft;
        gameObject.SetActive(true);
    }

    public virtual void Interact(string charID)
    {

        if(ActorsHit.Contains(charID))
        {
            return;
        }

        if (isPlayer)
        {
            ActorsHit.Add(charID);
        }

        if(IsContiniousDamage)
        {
            List<string> tempList = new List<string>();
            tempList.Add(charID);

            SocketClient.Instance.SendHitAbility(tempList, CurrentAbility.name);
        }

        WasHitThisFrame = true;
    }

    protected virtual void Shut()
    {
        if(ActorsHit.Count > 0 && !IsContiniousDamage)
        {
            SocketClient.Instance.SendHitAbility(ActorsHit, CurrentAbility.name);
            CurrentHitEvent.Invoke(CurrentAbility);
        }

        this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (timeLeftCurrent <= 0f)
        {
            Shut();
        }

        timeLeftCurrent -= 1f * Time.deltaTime;

        if(StickToOwner && CurrentOwner != null)
        {
            transform.position = CurrentOwner.CInstance.transform.position;
        }

        if(DependsOnMovementAbility && CurrentOwnerMovementController != null)
        {
            if(CurrentOwnerMovementController.Status.MovementAbilityRoutineInstance == null)
            {
                Shut();
            }
        }

        if(ShutOnHit && WasHitThisFrame)
        {
            WasHitThisFrame = false;
            Shut();
        }
    }

}
