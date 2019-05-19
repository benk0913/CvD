using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanelUI : MonoBehaviour
{
    public static InGamePanelUI Instance;

    [SerializeField]
    Transform AbilitiesGrid;

    MovementController Actor;

    [SerializeField]
    GameObject AbilityUIPrefab;

    [SerializeField]
    HealthBarUI HPBar;

    [SerializeField]
    AbilityTooltipUI AbilityTooltip;

    [SerializeField]
    CanvasGroup AbilityChargeCG;

    [SerializeField]
    Image AbilityChargeImage;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetInfo(MovementController actor)
    {
        this.gameObject.SetActive(true);

        this.Actor = actor;
        InitializeAbilities();
        HPBar.SetInfo(actor.Status.OnHPChanged);
    }

    #region Abilities

    void InitializeAbilities()
    {
        if(InitializeAbilitiesRoutineInstance != null)
        {
            return;
        }

        InitializeAbilitiesRoutineInstance = StartCoroutine(InitializeAbilitiesRoutine());
    }

    Coroutine InitializeAbilitiesRoutineInstance;
    IEnumerator InitializeAbilitiesRoutine()
    {
        ClearAbilities();

        while(Actor.Status == null)
        {
            yield return 0;
        }

        for (int i = 0; i < Actor.Status.AbilityStatuses.Count; i++)
        {
            AddAbility(Actor.Status.AbilityStatuses[i], i);
        }
        
        InitializeAbilitiesRoutineInstance = null;
    }

    void AddAbility(AbilityStatus abilityStatus, int index)
    {
        GameObject abilityObject = Instantiate(AbilityUIPrefab);
        abilityObject.transform.SetParent(AbilitiesGrid, false);

        abilityObject.GetComponent<AbilityIconUI>().SetInfo(abilityStatus, index);
    }

    void ClearAbilities()
    {
        for(int i=0;i<AbilitiesGrid.childCount;i++)
        {
            Destroy(AbilitiesGrid.GetChild(i).gameObject);
        }
    }

    public void ShowAbilityTooltip(Vector2 position, string title, string content, List<string> attributes = null)
    {
        AbilityTooltip.transform.position = position;
        AbilityTooltip.Show(title, content, attributes);
    }

    public void HideAbilityTooltip()
    {
        AbilityTooltip.Hide();
    }

    public void SetAbilityCharge(float value)
    {
        AbilityChargeImage.fillAmount = value;
    }

    public void ShowAbilityCharge()
    {
        if (VisibilityAbilityChargeRoutineInstance != null)
        {
            StopCoroutine(VisibilityAbilityChargeRoutineInstance);
        }

        VisibilityAbilityChargeRoutineInstance = StartCoroutine(ShowAbilityChargeRoutine());
    }

    public void HideAbilityCharge()
    {
        if (VisibilityAbilityChargeRoutineInstance != null)
        {
            StopCoroutine(VisibilityAbilityChargeRoutineInstance);
        }

        VisibilityAbilityChargeRoutineInstance = StartCoroutine(HideAbilityChargeRoutine());
    }

    Coroutine VisibilityAbilityChargeRoutineInstance;
    IEnumerator ShowAbilityChargeRoutine()
    {
        while (AbilityChargeCG.alpha < 1f)
        {
            AbilityChargeCG.alpha += 3f * Time.deltaTime;

            yield return 0;
        }

        VisibilityAbilityChargeRoutineInstance = null;
    }
    
    IEnumerator HideAbilityChargeRoutine()
    {
        while (AbilityChargeCG.alpha > 0f)
        {
            AbilityChargeCG.alpha -= 3f * Time.deltaTime;

            yield return 0;
        }

        VisibilityAbilityChargeRoutineInstance = null;
    }



    #endregion
}

