using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public Ability Reference;

    [SerializeField]
    Image CooldownFill;

    [SerializeField]
    GameObject CooldownPanel;

    [SerializeField]
    TextMeshProUGUI CooldownSecondsText;

    [SerializeField]
    TextMeshProUGUI AbilityHotkeyText;

    [SerializeField]
    Image Icon;


    public void Initialize(Ability ability, int AbilityNumber)
    {
        this.Reference = ability;

        AbilityHotkeyText.text = InputMap.Map["Ability" + (AbilityNumber+1)].ToString();

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
            t += (1f / Reference.Cooldown) * Time.deltaTime;

            CooldownSecondsText.text = Mathf.RoundToInt(t * Reference.Cooldown).ToString();
            CooldownFill.fillAmount = t;

            yield return 0;
        }

        CooldownPanel.SetActive(false);
        CooldownRoutineInstance = null;
    }
}
