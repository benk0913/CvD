using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[CreateAssetMenu(fileName = "Database", menuName = "DataObjects/Database", order = 2)]
public class Database : ScriptableObject
{
    public List<CharacterClass> Classes;

    public List<Buff> Buffs;

    public CharacterClass GetClass(string className)
    {
        for(int i=0;i<Classes.Count;i++)
        {
            if(Classes[i].name == className)
            {
                return Classes[i];
            }
        }

        return Classes[0]; //TOOD REPLACE WITH NULL WHEN CLASSES ARE IMPLEMENTED IN SERVER!
    }

    public Ability GetAbility(string abilityName)
    {
        Ability tempAbility = null;
        for(int i=0;i<Classes.Count;i++)
        {
            tempAbility = Classes[i].GetAbility(abilityName);
            if(tempAbility != null)
            {
                return tempAbility;
            }
        }

        return Classes[0].Abilities[0]; //TOOD REPLACE WITH NULL WHEN CLASSES ARE IMPLEMENTED IN SERVER!;
    }

    public Buff GetBuff(string buffName)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].name == buffName)
            {
                return Buffs[i];
            }
        }

        return Buffs[0]; //TOOD REPLACE WITH NULL WHEN BUFFS ARE IMPLEMENTED IN SERVER!;
    }

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        for (int i = 0; i < Classes.Count; i++)
        {
            node["classes"][i] = Classes[i].ToJson();
        }

        for (int i = 0; i < Buffs.Count; i++)
        {
            node["buffs"][i] = Buffs[i].ToJson();
        }

        return node;
    }
}
