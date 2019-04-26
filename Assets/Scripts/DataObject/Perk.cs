using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perk", menuName = "DataObjects/Content/Perk", order = 2)]
public class Perk : ScriptableObject
{
    public string DisplayName;
    public PerkAttribute Attribute;
    public float MinValue = 0f;
    public float MaxValue = 1f;

    public List<Perk> Perks;

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        node["perk_attribute"] = this.Attribute.name;
        node["min_value"] = this.MinValue.ToString();
        node["max_value"] = this.MaxValue.ToString();

        for (int i = 0; i < Perks.Count; i++)
        {
            node["perks"][i] = Perks[i].ToJson();
        }

        return node;
    }

    public Perk GetPerk(string perkKey)
    {
        for(int i=0;i<Perks.Count;i++)
        {
            if(Perks[i].name == perkKey)
            {
                return Perks[i];
            }
        }

        return null;
    }

    public Perk GetPerkByType(string perkType)
    {
        for (int i = 0; i < Perks.Count; i++)
        {
            if (Perks[i].Attribute.name == perkType)
            {
                return Perks[i];
            }
        }

        return null;
    }

    public float GetPerkValue(string perkKey, float defaultValue)
    {
        Perk tempPerk = GetPerk(perkKey);

        if(tempPerk == null)
        {
            return defaultValue;
        }

        return Random.Range(tempPerk.MinValue, tempPerk.MaxValue);
    }

    public float GetPerkValueByType(string perkType, float defaultValue)
    {
        Perk tempPerk = GetPerkByType(perkType);

        if (tempPerk == null)
        {
            return defaultValue;
        }

        return Random.Range(tempPerk.MinValue, tempPerk.MaxValue);
    }
}
