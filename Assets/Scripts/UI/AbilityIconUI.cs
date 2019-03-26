using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public AbilityStatus AbilityStatus;

    [SerializeField]
    Image CooldownFill;

    [SerializeField]
    GameObject CooldownPanel;

    [SerializeField]
    TextMeshProUGUI CooldownSecondsText;

    [SerializeField]
    TextMeshProUGUI AbilityHotkeyText;

    [SerializeField]
    TextMeshProUGUI AbilityChargesText;

    [SerializeField]
    Image Icon;


    public void Initialize(AbilityStatus ability, int AbilityNumber)
    {
        this.AbilityStatus = ability;

        AbilityHotkeyText.text = InputMap.Map["Ability" + (AbilityNumber+1)].ToString();

        if (ability.Reference.ChargesCap > 1)
        {
            AbilityChargesText.text = ability.Reference.ChargesCap.ToString();
        }

        CooldownPanel.SetActive(false);
    }

    public void ActivateAbility()
    {
        if(CooldownRoutineInstance != null)
        {
            StopCoroutine(CooldownRoutineInstance);
            CooldownPanel.SetActive(false);
        }

        CooldownRoutineInstance = StartCoroutine(CooldownRoutine());
    }
    

    Coroutine CooldownRoutineInstance;
    IEnumerator CooldownRoutine()
    {
        CooldownPanel.SetActive(true);

        float t = 0f;
        while(t < 1f)
        {
            t += (1f / AbilityStatus.Reference.Cooldown) * Time.deltaTime;

            CooldownSecondsText.text = Mathf.RoundToInt(AbilityStatus.Reference.Cooldown - (t * AbilityStatus.Reference.Cooldown)).ToString();
            CooldownFill.fillAmount = 1f-t;

            yield return 0;
        }

        CooldownPanel.SetActive(false);
        CooldownRoutineInstance = null;
    }
}
