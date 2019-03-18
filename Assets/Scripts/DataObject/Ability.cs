using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "DataObjects/Content/Ability", order = 2)]
public class Ability : ScriptableObject
{
    public string DisplayName;

    public float Cooldown = 1f;
    
    public int AnimationID = 0;

    public List<Perk> Perks = new List<Perk>();

}
