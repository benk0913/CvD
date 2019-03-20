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

    private void Awake()
    {
        Instance = this;
    }

    public void SetInfo(MovementController actor)
    {
        this.Actor = actor;
        InitializeAbilities();
    }

    #region Abilities

    void InitializeAbilities()
    {
        ClearAbilities();

        for(int i=0;i< Actor.Character.Class.Abilities.Count;i++)
        {
            AddAbility(Actor.Character.Class.Abilities[i], i);
        }
    }

    void AddAbility(Ability ability, int index)
    {
        GameObject abilityObject = Instantiate(AbilityUIPrefab);
        abilityObject.transform.SetParent(AbilitiesGrid, false);

        abilityObject.GetComponent<AbilityIconUI>().Initialize(ability, index);
    }

    void ClearAbilities()
    {
        for(int i=0;i<AbilitiesGrid.childCount;i++)
        {
            Destroy(AbilitiesGrid.GetChild(i).gameObject);
        }
    }

    public void ActivateAbility(Ability ability)
    {
        AbilityIconUI currentAbility;

        for(int i=0;i<AbilitiesGrid.childCount;i++)
        {
            currentAbility = AbilitiesGrid.GetChild(i).GetComponent<AbilityIconUI>();

            if (currentAbility.Reference.name == ability.name)
            {
                currentAbility.ActivateAbility();
            }
        }
    }

    #endregion
}

