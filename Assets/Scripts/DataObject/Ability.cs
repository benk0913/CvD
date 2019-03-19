using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "DataObjects/Content/Ability", order = 2)]
public class Ability : ScriptableObject
{
    public string DisplayName;

    public float Duration = 1f;

    public float Cooldown = 1f;

    public List<string> Animations;

    public List<Perk> Perks = new List<Perk>();

    public List<GameObject> ObjectsToSpawn;

    public Ability AbilityOnHit;

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        node["ability_key"] = this.name;
        node["duration"] = this.Duration.ToString();
        node["cooldown"] = this.Cooldown.ToString();

        for (int i = 0; i < Perks.Count; i++)
        {
            node["perks"][i] = Perks[i].ToJson();
        }

        if(AbilityOnHit != null)
        {
            node["ability_on_hit"] = this.AbilityOnHit.ToJson();
        }


        return node;
    }

}
