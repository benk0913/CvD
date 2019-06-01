using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxProjectileHomed : HitboxProjectile
{
    Transform homingTarget;

    [SerializeField]
    float RotationSpeed = 1f;

    protected void LateUpdate()
    {
        UpdateMovement();
    }


    public override void SetInfo(CharacterInfo ownerInsance, Ability ability, HitboxEvent onHitEvent, bool isplayer)
    {
        base.SetInfo(ownerInsance, ability, onHitEvent, isplayer);
        homingTarget = ownerInsance.CInstance.GetComponent<MovementController>().LastTargets[0].CInstance.transform;
    }
    
    protected override void UpdateMovement()
    {

        base.UpdateMovement();

        if (homingTarget == null)
        {
            return;
        }

        Vector3 dir = homingTarget.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * RotationSpeed);

    }

}
