using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo
{
    public Scene SceneReference;

    public SceneSettings Settings;

    public List<CharacterInfo> Characters = new List<CharacterInfo>();

    public CharacterInfo GetPlayer(string id)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].ID == id)
            {
                return Characters[i];
            }
        }

        return null;
    }

    public CharacterInfo GetPlayer(GameObject actorObj)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].CInstance == actorObj)
            {
                return Characters[i];
            }
        }

        return null;
    }

}
