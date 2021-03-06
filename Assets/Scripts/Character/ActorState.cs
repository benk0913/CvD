﻿using System.Collections;
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

    public HPChangedEvent OnHPChanged = new HPChangedEvent();

    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
        set
        {
            currentHP = value;
            OnHPChanged.Invoke(currentHP, CurrentCharacter.Class.BaseHP);
        }
    }
    int currentHP;


    public void Initialize(CharacterInfo character)
    {
        this.CurrentCharacter = character;
        this.HPBar.SetInfo(OnHPChanged);

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
    public Coroutine LastCooldownRoutine;
    public UnityEvent OnRecharge = new UnityEvent();
    public CooldownUpdateEvent OnCooldownUpdated = new CooldownUpdateEvent();
    public UnityEvent OnCooldownComplete = new UnityEvent();
    public UnityEvent OnAbilityActivated = new UnityEvent();
    public int Charges;
    public float CooldownSecondsLeft;

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

    public void UpdateCooldown(float secondsLeft, bool external = false)
    {
        this.CooldownSecondsLeft = secondsLeft;
        OnCooldownUpdated.Invoke(external);

        if(this.CooldownSecondsLeft <= 0)
        {
            CompleteCooldown(null);
        }
    }

    public void CompleteCooldown(Coroutine routineInstance)
    {
        Recharge();
        
        if(routineInstance == LastCooldownRoutine)
        {
            LastCooldownRoutine = null;
        }

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
        this.LastCooldownRoutine = cooldownRoutine;
    }
}

public class BuffStatus
{
    public Buff Reference;

    public UnityEvent OnClearEvent = new UnityEvent();

    public GameObject BuffPrefab;

    public BuffStatus(Buff reference, GameObject buffPrefab = null)
    {
        this.Reference = reference;
        this.BuffPrefab = buffPrefab;
    }
}

public class HPChangedEvent : UnityEvent<int, int>
{
}

public class CooldownUpdateEvent : UnityEvent<bool>
{

}