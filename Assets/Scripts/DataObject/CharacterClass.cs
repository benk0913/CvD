using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "DataObjects/Content/Class", order = 2)]
public class CharacterClass : ScriptableObject
{
    public string DisplayName;

    public GameObject ClassActor;

    public int BaseHP = 10;

    public List<string> HurtAnimations = new List<string>();

    public List<string> DeathAnimations = new List<string>();

    public List<Ability> Abilities = new List<Ability>();

    public Ability GetAbility(string abilityName)
    {
        for(int i=0;i<Abilities.Count;i++)
        {
            if(Abilities[i].name == abilityName)
            {
                return Abilities[i];
            }
        }

        return null;
    }

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        node["class_key"] = this.name;
        node["base_hp"] = this.BaseHP.ToString();
        
        for(int i=0;i<Abilities.Count;i++)
        {
            node["abilities"][i] = Abilities[i].ToJson();
        }

        return node;
    }
}
