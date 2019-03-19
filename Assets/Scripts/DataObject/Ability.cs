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

}
