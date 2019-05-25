using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public float CurrentValue;

    [SerializeField]
    Image FillImage;

    [SerializeField]
    public bool HasScaleAnimation = true;

    [SerializeField]
    TextMeshProUGUI PrecentText;

    HPChangedEvent OnHPChangedEvent;

    Color InitColor;
    
    public void SetInfo(HPChangedEvent onHpChangedEvent)
    {
        if(OnHPChangedEvent != null)
        {
            OnHPChangedEvent.RemoveListener(OnHPChanged);
        }

        OnHPChangedEvent = onHpChangedEvent;

        OnHPChangedEvent.AddListener(OnHPChanged);

        RefreshValue(1f);

        InitColor = FillImage.color;
    }

    private void OnHPChanged(int newHP, int totalHP)
    {
        RefreshValue((float)newHP / (float)totalHP);
    }

    public void RefreshValue(float value)
    {

        if(PrecentText != null)
        {
            PrecentText.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        this.CurrentValue = value;

        if(RefreshRoutineInstance != null)
        {
            StopCoroutine(RefreshRoutineInstance);
        }

        RefreshRoutineInstance = StartCoroutine(RefreshRoutine());
    }

    Coroutine RefreshRoutineInstance;
    IEnumerator RefreshRoutine()
    {
        float t = 0f;
        while (t<1f)
        {
            if (HasScaleAnimation)
            {
                if (t < 0.333f)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.5f, t * 18f);
                }
                else
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, (t-0.3f)*3.9f);
                }
            }
            
            if(t<0.05f)
            {
                FillImage.color = Color.Lerp(FillImage.color, Color.black, t*10f);

                yield return 0;
            }
            else if (t < 0.2f)
            {
                FillImage.color = Color.Lerp(Color.black, InitColor, (t*5f) - 0.05f);
            }

            t += 1f * Time.deltaTime;

            FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, CurrentValue, t);

            yield return 0;
        }

        transform.localScale = Vector3.one;

       RefreshRoutineInstance = null;
    }
}
