using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxScript : MonoBehaviour {

    [SerializeField]
    float timeLeft = 0.1f;

    [SerializeField]
    bool ShutOnHit = true;

    [System.NonSerialized]
    public string ParentOwner;

    [System.NonSerialized]
    public Ability ParentAbility;

    [System.NonSerialized]
    public List<string> ActorsHit = new List<string>();

    public void SetInfo(string owner ,Ability ability)
    {
        this.ParentAbility = ability;
        this.ParentOwner = owner;

        timeLeft = 0.1f;
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
            SocketClient.Instance.SendHitAbility(ActorsHit, ParentAbility.name);
        }

        this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (timeLeft <= 0f)
        {
            this.gameObject.SetActive(false);
        }

        timeLeft -= 1f * Time.deltaTime;
    }

}
