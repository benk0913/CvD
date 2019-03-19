using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InputMap : MonoBehaviour {

    [SerializeField]
    List<InputKeyPair> InitList = new List<InputKeyPair>();
     
    public static Dictionary<string, KeyCode> Map = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        Map.Clear();

        for (int i = 0; i < InitList.Count; i++)
        {
            Map.Add(InitList[i].name, InitList[i].KeyCode);
        }

        LoadMap();
    }

    public static void LoadMap()
    {
        string currentKey;
        int currentValue;

        int i = 0;
        while (PlayerPrefs.HasKey("inputMapKey_" + i))
        {
            currentKey = PlayerPrefs.GetString("inputMapKey_" + i);
            currentValue = PlayerPrefs.GetInt("inputMapValue_" + i);

            if (Map.ContainsKey(currentKey))
            {
                Map[currentKey] = (KeyCode)currentValue;
            }
            else
            {
                Map.Add(currentKey, (KeyCode)currentValue);
            }

            i++;
        }
    }

    public static void SaveMap()
    {
        for (int i = 0; i < Map.Keys.Count; i++)
        {
            PlayerPrefs.SetString("inputMapKey_" + i, Map.Keys.ElementAt(i));
            PlayerPrefs.SetInt("inputMapValue_" + i, (int) Map[Map.Keys.ElementAt(i)]);
        }

        PlayerPrefs.Save();
    }

}