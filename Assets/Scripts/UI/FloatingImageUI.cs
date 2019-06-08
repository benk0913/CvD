using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingImageUI : MonoBehaviour //TODO Merge with FloatingDamageUI (code duplication)
{
    [SerializeField]
    SpriteRenderer iconImage;

    [SerializeField]
    Color fromColor;

    [SerializeField]
    Color toColor;

    [SerializeField]
    float Duration = 2f;

    public void Activate()
    {
        StartCoroutine(EffectRoutine());
    }

    public void Activate(Sprite sSprite)
    {

        iconImage.sprite = sSprite;

        StartCoroutine(EffectRoutine());
    }

    public void Activate(string spriteKey)
    {
        Activate(CORE.Instance.SpritesDatabase[spriteKey]);
    }

    IEnumerator EffectRoutine()
    {

        float t = 0f;
        while (t < 1f)
        {
            t += (1f / Duration) * Time.deltaTime;

            iconImage.color = Color.Lerp(fromColor, toColor, t);
            transform.position += transform.up * (1f - t) * Time.deltaTime;

            yield return 0;
        }

        this.gameObject.SetActive(false);
    }
}
