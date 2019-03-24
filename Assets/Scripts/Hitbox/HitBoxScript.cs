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

    bool isPlayer;

    public virtual void SetInfo(CharacterInfo ownerInsance,Ability ability, HitboxEvent onHitEvent, bool isplayer)
    {
        this.isPlayer = isplayer;

        ActorsHit.Clear();
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

        if (ShutOnHit)
        {
            Shut();
        }
    }

    protected virtual void Shut()
    {

        if(ActorsHit.Count > 0)
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
            this.gameObject.SetActive(false);
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
    }

}
