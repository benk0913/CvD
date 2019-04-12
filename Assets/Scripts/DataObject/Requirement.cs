using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Requirement", menuName = "DataObjects/Content/Requirement", order = 2)]
public class Requirement : ScriptableObject
{
    public PerkAttribute Attribute;
    public float MinValue = 0f;
    public float MaxValue = 1f;

    public List<Requirement> SubRequirements;

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        node["requirement_attribute"] = this.Attribute.name;
        node["min_value"] = this.MinValue.ToString();
        node["max_value"] = this.MaxValue.ToString();

        for (int i = 0; i < SubRequirements.Count; i++)
        {
            node["requirements"][i] = SubRequirements[i].ToJson();
        }

        return node;
    }

    public Requirement GetRequirement(string requirementKey)
    {
        for (int i = 0; i < SubRequirements.Count; i++)
        {
            if (SubRequirements[i].name == requirementKey)
            {
                return SubRequirements[i];
            }
        }

        return null;
    }

    public Requirement GetRequirementByType(string requirementType)
    {
        for (int i = 0; i < SubRequirements.Count; i++)
        {
            if (SubRequirements[i].Attribute.name == requirementType)
            {
                return SubRequirements[i];
            }
        }

        return null;
    }

    public float GetRequirementValue(string requirementKey, float defaultValue)
    {
        Requirement tempPerk = GetRequirement(requirementKey);

        if (tempPerk == null)
        {
            return defaultValue;
        }

        return Random.Range(tempPerk.MinValue, tempPerk.MaxValue);
    }

    public float GetPerkValueByType(string perkType, float defaultValue)
    {
        Requirement tempPerk = GetRequirementByType(perkType);

        if (tempPerk == null)
        {
            return defaultValue;
        }

        return Random.Range(tempPerk.MinValue, tempPerk.MaxValue);
    }
}
