using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxProjectile : HitBoxScript
{
    [SerializeField]
    protected float Speed = 3f;

    [SerializeField]
    protected Rigidbody2D Rigid;

    private void LateUpdate()
    {
        Rigid.position += (Vector2) transform.right * (-transform.localScale.x) * Speed * Time.deltaTime;
        
        if (timeLeftCurrent <= 0f)
        {
            this.gameObject.SetActive(false);
        }

        timeLeftCurrent -= 1f * Time.deltaTime;

        if (StickToOwner && CurrentOwner != null)
        {
            transform.position = CurrentOwner.CInstance.transform.position;
        }

        if (DependsOnMovementAbility && CurrentOwnerMovementController != null)
        {
            if (CurrentOwnerMovementController.Status.MovementAbilityRoutineInstance == null)
            {
                Shut();
            }
        }

        if (ShutOnHit && WasHitThisFrame)
        {
            WasHitThisFrame = false;
            Shut();
        }
    }
}
