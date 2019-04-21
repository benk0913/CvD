using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void ShowAbilityTooltip(Vector2 position, string title, string content)
    {
        AbilityTooltip.transform.position = position;
        AbilityTooltip.Show(title, content);
    }

    public void HideAbilityTooltip()
    {
        AbilityTooltip.Hide();
    }

    #endregion
}

