using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class InputMap {

    [SerializeField]
    List<InputKeyPair> InitList = new List<InputKeyPair>();
     
    public static Dictionary<string, KeyCode> Map = new Dictionary<string, KeyCode>();

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
        int i = 0;
        while (PlayerPrefs.HasKey("inputMapKey_" + i))
        {
            if (Map.ContainsKey(PlayerPrefs.GetString("inputMapKey_" + i)))
            {
                Map[PlayerPrefs.GetString("inputMapKey_" + i)] = (KeyCode)PlayerPrefs.GetInt("inputMapValue_" + i);
            }
            else
            {
                Map.Add(PlayerPrefs.GetString("inputMapKey_" + i), (KeyCode)PlayerPrefs.GetInt("inputMapValue_" + i));
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