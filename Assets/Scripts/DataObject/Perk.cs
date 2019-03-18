using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perk", menuName = "DataObjects/Content/Perk", order = 2)]
public class Perk : ScriptableObject
{
    public PerkAttribute Attribute;
    public float MinValue = 0f;
    public float MaxValue = 1f;
    public float Chance = 1f;
}
