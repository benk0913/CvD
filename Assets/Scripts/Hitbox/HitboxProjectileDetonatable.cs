using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxProjectileDetonatable : HitboxProjectile
{

    [SerializeField]
    List<GameObject> DetonationObjects;

    public override void SetInfo(CharacterInfo ownerInsance, Ability ability, HitboxEvent onHitEvent)
    {
        base.SetInfo(ownerInsance, ability, onHitEvent);
    }

    public override void Detonate()
    {
        foreach (GameObject obj in DetonationObjects)
        {
            GameObject generatedObject = ResourcesLoader.Instance.GetRecycledObject(obj);
            generatedObject.transform.position = transform.position;

            HitBoxScript hitbox = generatedObject.GetComponent<HitBoxScript>();
            if (hitbox != null)
            {
                hitbox.SetInfo(this.CurrentOwner, this.CurrentAbility, this.CurrentHitEvent);
            }
        }

        Shut();
    }
}
