using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTeleport : HitBoxScript
{

    public override void SetInfo(CharacterInfo ownerInsance, Ability ability, HitboxEvent onHitEvent)
    {
        base.SetInfo(ownerInsance, ability, onHitEvent);
        CurrentOwner.CInstance.transform.position = transform.position;
        Shut();
    }
}
