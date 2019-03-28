using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public AbilityStatus abilityStatus;

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
        if(this.abilityStatus != null)
        {
            RemoveListeners();
        }

        this.abilityStatus = ability;

        AddListeners();

        AbilityHotkeyText.text = InputMap.Map["Ability" + (AbilityNumber+1)].ToString();

        if (ability.Reference.ChargesCap > 1)
        {
            AbilityChargesText.text = "x"+ability.Reference.ChargesCap.ToString();
        }

        CooldownPanel.SetActive(false);
    }

    void AddListeners()
    {
        this.abilityStatus.OnAbilityActivated.AddListener(OnAbilityActivated);
        this.abilityStatus.OnCooldownComplete.AddListener(OnAbilityCooldownComplete);
        this.abilityStatus.OnRecharge.AddListener(OnAbilityRecharge);
    }

    void RemoveListeners()
    {
        this.abilityStatus.OnAbilityActivated.RemoveListener(OnAbilityActivated);
        this.abilityStatus.OnCooldownComplete.RemoveListener(OnAbilityCooldownComplete);
        this.abilityStatus.OnRecharge.RemoveListener(OnAbilityRecharge);
    }

    private void OnAbilityRecharge()
    {
        if(this.abilityStatus.Reference.ChargesCap <= 1)
        {
            AbilityChargesText.text = "";
            return;
        }

        AbilityChargesText.text = "x"+this.abilityStatus.Charges.ToString();
    }

    private void OnAbilityCooldownComplete()
    {
        CooldownComplete();
    }

    private void OnAbilityActivated()
    {
        ActivateAbility();
    }

    void ActivateAbility()
    {
        if(CooldownRoutineInstance != null)
        {
            StopCoroutine(CooldownRoutineInstance);
            CooldownPanel.SetActive(false);
        }

        if (abilityStatus.Reference.ChargesCap > 1)
        {
            AbilityChargesText.text = "x" + abilityStatus.Charges;
        }
        else
        {
            AbilityChargesText.text = "";
        }

        CooldownRoutineInstance = StartCoroutine(CooldownRoutine());
    }

    void CooldownComplete()
    {
        if(CooldownRoutineInstance == null)
        {

            CooldownPanel.SetActive(false);
        }
    }
    

    Coroutine CooldownRoutineInstance;
    IEnumerator CooldownRoutine()
    {
        CooldownPanel.SetActive(true);

        float t = 0f;
        while(t < 1f)
        {
            t += (1f / abilityStatus.Reference.Cooldown) * Time.deltaTime;

            CooldownSecondsText.text = Mathf.RoundToInt(abilityStatus.Reference.Cooldown - (t * abilityStatus.Reference.Cooldown)).ToString();
            CooldownFill.fillAmount = 1f-t;

            yield return 0;
        }

        CooldownRoutineInstance = null;
        CooldownComplete();
    }
}
