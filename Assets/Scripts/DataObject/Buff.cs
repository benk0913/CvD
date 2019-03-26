﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "DataObjects/Content/Buff", order = 2)]
public class Buff : ScriptableObject
{
    public string DisplayName;
    public float Duration;
    public List<Perk> Perks;
    public GameObject BuffPrefab;

    public JSONNode ToJson()
    {
        JSONNode node = new JSONClass();

        node["buff_key"] = this.name;
        node["duration"] = this.Duration.ToString();

        for (int i = 0; i < Perks.Count; i++)
        {
            node["perks"][i] = Perks[i].ToJson();
        }

        return node;
    }
}