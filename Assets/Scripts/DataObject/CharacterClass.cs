using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "DataObjects/Content/Class", order = 2)]
public class CharacterClass : ScriptableObject
{
    public string DisplayName;

    public List<Ability> Abilities = new List<Ability>();
}
