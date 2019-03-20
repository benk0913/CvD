using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorState
{
    CharacterInfo CurrentCharacter;

    [SerializeField]
    HealthBarUI HPBar;

    List<AbilityStatus> AbilityStatuses = new List<AbilityStatus>();

    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
        set
        {
            currentHP = value;
            HPBar.RefreshValue((float)currentHP / (float)CurrentCharacter.Class.BaseHP);
        }
    }
    int currentHP;


    public void Initialize(CharacterInfo character)
    {
        this.CurrentCharacter = character;

        CurrentHP = character.Class.BaseHP;

        for(int i=0;i<CurrentCharacter.Class.Abilities.Count;i++)
        {
            AbilityStatuses.Add(new AbilityStatus(CurrentCharacter.Class.Abilities[i]));
        }
    }

    public AbilityStatus GetAbilityStatus(Ability ability)
    {
        for(int i=0;i<AbilityStatuses.Count;i++)
        {
            if (AbilityStatuses[i].Reference.name == ability.name)
            {
                return AbilityStatuses[i];
            }
        }

        return null;
    }

}

public class AbilityStatus
{
    public Ability Reference;
    public Coroutine CooldownRoutine;

    public AbilityStatus(Ability ability)
    {
        this.Reference = ability;
    }
}