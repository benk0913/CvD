using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpawner : MonoBehaviour
{
    [SerializeField]
    List<GameObject> ObjectsToSpawn;

    [SerializeField]
    bool myDirection;

    public void Spawn(string objectToSpawn)
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(GetObjectByName(objectToSpawn));
        tempObj.transform.position = transform.position;

        if (myDirection)
        {
            tempObj.transform.localScale =
                new Vector3(
                    Mathf.Abs(tempObj.transform.localScale.x) * ((transform.localScale.x < 0)? -1f : 1f),
                    tempObj.transform.localScale.y,
                    tempObj.transform.localScale.z);
        }
    }

    GameObject GetObjectByName(string objName)
    {
        for(int i=0;i<ObjectsToSpawn.Count;i++)
        {
            if (ObjectsToSpawn[i].name == objName)
            {
                return ObjectsToSpawn[i];
            }
        }

        return null;
    }
}
