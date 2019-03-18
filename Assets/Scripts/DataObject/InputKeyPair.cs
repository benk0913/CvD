using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "KeyPreset", menuName = "DataObjects/Input/KeyPreset", order = 2)]
public class InputKeyPair : ScriptableObject
{
    [SerializeField]
    public KeyCode KeyCode;

}
