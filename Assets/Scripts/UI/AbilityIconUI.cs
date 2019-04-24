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
    Button AbilityButton;

    [SerializeField]
    Image Icon;

    [SerializeField]
    Animator AbilityAnimator;


    public void SetInfo(AbilityStatus ability, int AbilityNumber)
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
        else
        {
            AbilityChargesText.text = "";
        }

        AbilityButton.interactable = !(abilityStatus.Charges <= 0);

        if(ability.Reference.Icon != null)
        {
            this.Icon.sprite = ability.Reference.Icon;
        }

        if(ability.CooldownSecondsLeft > 0)
        {
            CooldownPanel.SetActive(true);

            SetCooldownUI(
              Mathf.RoundToInt(abilityStatus.CooldownSecondsLeft).ToString()
            , (abilityStatus.CooldownSecondsLeft / abilityStatus.Reference.Cooldown));
        }
        else
        {
            CooldownPanel.SetActive(false);
        }
    }

    public void ShowTooltip()
    {
        List<string> attributes = new List<string>();

        attributes.Add("Cooldown : " + abilityStatus.Reference.Cooldown);
        attributes.Add("Perks On Player: ");
        for (int i=0;i<abilityStatus.Reference.Perks.Count;i++)
        {
            attributes.Add(abilityStatus.Reference.Perks[i].name.ToString());
        }

        attributes.Add("Perks On Target: ");
        for (int i = 0; i < abilityStatus.Reference.PerksOnHit.Count; i++)
        {
            attributes.Add(abilityStatus.Reference.PerksOnHit[i].name.ToString());
        }

        InGamePanelUI.Instance.ShowAbilityTooltip(
            transform.position,
            this.abilityStatus.Reference.DisplayName,
            this.abilityStatus.Reference.Description,
            attributes);
    }

    public void HideTooltip()
    {
        InGamePanelUI.Instance.HideAbilityTooltip();
    }

    void AddListeners()
    {
        this.abilityStatus.OnAbilityActivated.AddListener(OnAbilityActivated);
        this.abilityStatus.OnCooldownUpdated.AddListener(OnAbilityCooldownUpdated);
        this.abilityStatus.OnCooldownComplete.AddListener(OnAbilityCooldownComplete);
        this.abilityStatus.OnRecharge.AddListener(OnAbilityRecharge);
    }

    void RemoveListeners()
    {
        this.abilityStatus.OnAbilityActivated.RemoveListener(OnAbilityActivated);
        this.abilityStatus.OnCooldownUpdated.RemoveListener(OnAbilityCooldownUpdated);
        this.abilityStatus.OnCooldownComplete.RemoveListener(OnAbilityCooldownComplete);
        this.abilityStatus.OnRecharge.RemoveListener(OnAbilityRecharge);
    }

    private void OnAbilityRecharge()
    {
        AbilityButton.interactable = !(abilityStatus.Charges <= 0);

        if (this.abilityStatus.Reference.ChargesCap <= 1)
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

    private void OnAbilityCooldownUpdated(bool external = false)
    {
        SetCooldownUI(
              Mathf.RoundToInt(abilityStatus.CooldownSecondsLeft).ToString()
            , (abilityStatus.CooldownSecondsLeft / abilityStatus.Reference.Cooldown)
            ,external);
    }

    private void OnAbilityActivated()
    {
        ActivateAbility();
    }

    void ActivateAbility()
    {
        if (abilityStatus.Reference.ChargesCap > 1)
        {
            AbilityChargesText.text = "x" + abilityStatus.Charges;
        }
        else
        {
            AbilityChargesText.text = "";
        }

        AbilityButton.interactable = !(abilityStatus.Charges <= 0);

        SetCooldownUI(Mathf.RoundToInt(abilityStatus.Reference.Cooldown).ToString(), 1f);

        AbilityAnimator.SetTrigger("AbilityUsedEffect");
    }

    void CooldownComplete()
    {
        if(AnimateCooldownUIRoutineInstance != null)
        {
            StopCoroutine(AnimateCooldownUIRoutineInstance);
            AnimateCooldownUIRoutineInstance = null;
        }

        AbilityAnimator.SetTrigger("CooldownCompleteEffect");

        CooldownPanel.SetActive(false);
    }
    
    void SetCooldownUI(string text, float fill, bool animated = false)
    {
        if(!CooldownPanel.activeInHierarchy)
        {
            CooldownPanel.SetActive(true);
        }

        if (!animated)
        {
            CooldownSecondsText.text = text;
            CooldownFill.fillAmount = fill;
            return;
        }

        if(AnimateCooldownUIRoutineInstance != null)
        {
            StopCoroutine(AnimateCooldownUIRoutineInstance);
        }

        AnimateCooldownUIRoutineInstance = StartCoroutine(AnimateCooldownUIRoutine(text, fill));

        AbilityAnimator.SetTrigger("CooldownEffect");
    }

    Coroutine AnimateCooldownUIRoutineInstance;
    IEnumerator AnimateCooldownUIRoutine(string text, float fill)
    {
        CooldownPanel.SetActive(true);
        
        CooldownSecondsText.text = text;

        float t = 0f;
        while (t < 1f)
        {
            t += 1f * Time.deltaTime;

            CooldownFill.fillAmount = Mathf.Lerp(CooldownFill.fillAmount, fill, t);

            yield return 0;
        }

        AnimateCooldownUIRoutineInstance = null;
    }


    //--DEPRECATED
    Coroutine CooldownRoutineInstance;
    IEnumerator CooldownRoutine()
    {
        CooldownPanel.SetActive(true);

        float t = 0f;
        while(t < 1f)
        {
            t += (1f / abilityStatus.Reference.Cooldown) * Time.deltaTime;

            SetCooldownUI(
                Mathf.RoundToInt(abilityStatus.Reference.Cooldown - (t * abilityStatus.Reference.Cooldown)).ToString()
                ,1f-t);

            yield return 0;
        }

        CooldownRoutineInstance = null;
        CooldownComplete();
    }
    //--
}
