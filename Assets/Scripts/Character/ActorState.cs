using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ActorState
{
    CharacterInfo CurrentCharacter;

    [SerializeField]
    HealthBarUI HPBar;

    public List<AbilityStatus> AbilityStatuses = new List<AbilityStatus>();

    public List<BuffStatus> ActiveBuffs = new List<BuffStatus>();

    public Coroutine MovementAbilityRoutineInstance;

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

    public BuffStatus GetBuffStatus(Buff buff)
    {
        for(int i=0;i<ActiveBuffs.Count;i++)
        {
            if(ActiveBuffs[i].Reference.name == buff.name)
            {
                return ActiveBuffs[i];
            }
        }

        return null;
    }

}

public class AbilityStatus
{
    public Ability Reference;
    public Coroutine CooldownRoutine;
    public UnityEvent OnRecharge = new UnityEvent();
    public UnityEvent OnCooldownComplete = new UnityEvent();
    public UnityEvent OnAbilityActivated = new UnityEvent();
    public int Charges;

    public AbilityStatus(Ability ability)
    {
        this.Reference = ability;
        Charges = ability.ChargesCap;
    }

    public void Recharge()
    {
        if (Charges >= Reference.ChargesCap)
        {
            return;
        }

        Charges++;

        OnRecharge.Invoke();
    }

    public void CooldownComplete()
    {
        Recharge();

        CooldownRoutine = null;

        OnCooldownComplete.Invoke();
    }

    public void ActivateAbility(Coroutine cooldownRoutine)
    {
        Charges--;

        StartRechargeCooldown(cooldownRoutine);

        OnAbilityActivated.Invoke();
    }

    public void StartRechargeCooldown(Coroutine cooldownRoutine)
    {
        this.CooldownRoutine = cooldownRoutine;
    }
}

public class BuffStatus
{
    public Buff Reference;

    public UnityEvent OnClearEvent;

    public GameObject BuffPrefab;

    public BuffStatus(Buff reference, GameObject buffPrefab = null)
    {
        this.Reference = reference;
        this.BuffPrefab = buffPrefab;
    }
}