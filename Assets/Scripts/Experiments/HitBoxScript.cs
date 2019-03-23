using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBoxScript : MonoBehaviour {

    [SerializeField]
    float timeLeft = 0.1f;

    float timeLeftCurrent;

    [SerializeField]
    bool ShutOnHit = true;

    [SerializeField]
    bool StickToOwner = false;

    [System.NonSerialized]
    public Ability CurrentAbility;

    [System.NonSerialized]
    public List<string> ActorsHit = new List<string>();

    [System.NonSerialized]
    public CharacterInfo CurrentOwner;

    HitboxEvent CurrentHitEvent;

    public void SetInfo(CharacterInfo ownerInsance,Ability ability, HitboxEvent onHitEvent)
    {
        ActorsHit.Clear();
        this.CurrentAbility = ability;
        this.CurrentOwner = ownerInsance;
        this.CurrentHitEvent = onHitEvent;

        timeLeftCurrent = timeLeft;
        gameObject.SetActive(true);
    }

    public void Interact(string charID)
    {
        if(ActorsHit.Contains(charID))
        {
            return;
        }

        ActorsHit.Add(charID);

        if (ShutOnHit)
        {
            Shut();
        }
    }

    void Shut()
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
    }

}
