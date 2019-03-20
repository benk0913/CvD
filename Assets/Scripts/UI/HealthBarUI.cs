using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public float CurrentValue;

    [SerializeField]
    Image FillImage;
    

    public void RefreshValue(float value)
    {
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
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, CurrentValue, t);

            yield return 0;
        }

        RefreshRoutineInstance = null;
    }
}
