using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxProjectileDetonatable : HitboxProjectile
{

    [SerializeField]
    GameObject DetonationHitbox;

    public override void SetInfo(CharacterInfo ownerInsance, Ability ability, HitboxEvent onHitEvent, bool isplayer)
    {
        base.SetInfo(ownerInsance, ability, onHitEvent, isplayer);
    }

    public override void Detonate()
    {
        GameObject tempHitbox = ResourcesLoader.Instance.GetRecycledObject(DetonationHitbox);

        tempHitbox.transform.position = transform.position;

        tempHitbox.GetComponent<HitBoxScript>().SetInfo(this.CurrentOwner, this.CurrentAbility, this.CurrentHitEvent, this.isPlayer);

        Shut();
    }
}
