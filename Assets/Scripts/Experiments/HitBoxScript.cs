using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxScript : MonoBehaviour {

    [SerializeField]
    float timeLeft = 0.1f;

    public string ownerID;

    public void SetInfo()
    {
        timeLeft = 0.1f;
        gameObject.SetActive(true);
    }

    public void Interact()
    {
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
