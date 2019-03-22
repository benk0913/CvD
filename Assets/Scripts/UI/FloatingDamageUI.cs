using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingDamageUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    Color fromColor;

    [SerializeField]
    Color toColor;

    [SerializeField]
    float Duration = 2f;
    
    public void Activate(string text)
    {

        Label.text = text;

        StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {

        float t = 0f;
        while(t<1f)
        {
            t += (1f/Duration) * Time.deltaTime;

            Label.color = Color.Lerp(fromColor, toColor, t);
            transform.position += transform.up * (1f - t) * Time.deltaTime;

            yield return 0;
        }

        this.gameObject.SetActive(false);
    }

    
}
