using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDetonator : MonoBehaviour
{
    List<Collider2D> detonated = new List<Collider2D>();

    [SerializeField]
    float TimeAlive = 1f;

    float currentTime = 1f;

    private void OnEnable()
    {
        detonated.Clear();
        currentTime = TimeAlive;
    }

    private void LateUpdate()
    {
        if(currentTime > 0f)
        {
            currentTime -= 1f * Time.deltaTime;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!detonated.Contains(collision) && collision.tag == "Detonatable")
        {
            detonated.Add(collision);

            HitBoxScript hitbox = collision.GetComponent<HitBoxScript>();

            hitbox.Detonate();
        }
    }
}
