using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "DataObjects/Content/Ability", order = 2)]
public class Ability : ScriptableObject
{
    public string DisplayName;

    [TextArea(2,6)]
    public string Description;

    public Sprite Icon;

    public float Duration = 1f;

    public int ChargesCap = 1;

    public float Cooldown = 1f;

    public bool StartWithCooldown = false;

    public bool CooldownResetsOnRespawn = true;

    public bool ImmobileCasting = false;

    public List<Requirement> CooldownRequirements = new List<Requirement>();

    public List<string> Animations;

    public List<Perk> Perks = new List<Perk>();
    
    public List<Perk> PerksOnHitTarget = new List<Perk>();

    public List<GameObject> ObjectsToSpawn;

    public List<GameObject> ObjectsToSpawnOnTargets;

    [Tooltip("Ability which gets executed when this one hits.")]
    public Ability AbilityOnHit;

    [Tooltip("Is there going to be a different ability when facing left? (Used for abilities based on direction / etc...)")]
    public Ability AbilityOnLeft;

    [Tooltip("Is there going to be a different ability when falling? (Used for abilities based on direction / etc...)")]
    public Ability AbilityOnFalling;

    public JSONNode ToJson()
    {
        //Debug.Log(this.name);

        JSONNode node = new JSONClass();

        node["ability_key"] = this.name;
        node["duration"] = this.Duration.ToString();
        node["cooldown"] = this.Cooldown.ToString();
        node["start_with_cooldown"] = this.StartWithCooldown.ToString();
        node["cooldown_resets_on_respawn"] = this.CooldownResetsOnRespawn.ToString();

        for (int i = 0; i < CooldownRequirements.Count; i++)
        {
            node["cooldown_requirements"][i] = CooldownRequirements[i].ToJson();
        }

        for (int i = 0; i < Perks.Count; i++)
        {
            node["perks"][i] = Perks[i].ToJson();
        }

        for (int i = 0; i < PerksOnHitTarget.Count; i++)
        {
            node["perks_on_hit"][i] = PerksOnHitTarget[i].ToJson();
        }

        if (AbilityOnHit != null)
        {
            node["ability_on_hit"] = this.AbilityOnHit.ToJson();
        }

        if (AbilityOnLeft != null)
        {
            node["ability_on_left"] = this.AbilityOnLeft.ToJson();
        }

        if (AbilityOnFalling != null)
        {
            node["ability_on_falling"] = this.AbilityOnFalling.ToJson();
        }
        
        return node;
    }

    public bool HasTimerCountdown
    {
        get
        {
            return CooldownRequirements.Count == 0;
        }
    }

    public Ability GetAbility(string abilityKey)
    {
        if(this.name == abilityKey)
        {
            return this;
        }

        if(AbilityOnHit != null && AbilityOnHit.name == abilityKey)
        {
            return AbilityOnHit;
        }

        if (AbilityOnLeft != null && AbilityOnLeft.name == abilityKey)
        {
            return AbilityOnLeft;
        }

        if (AbilityOnFalling != null && AbilityOnFalling.name == abilityKey)
        {
            return AbilityOnFalling;
        }

        return null;
    }

}
