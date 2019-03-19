﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "DataObjects/Database", order = 2)]
public class Database : ScriptableObject
{
    [SerializeField]
    public List<CharacterClass> Classes;

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
}
