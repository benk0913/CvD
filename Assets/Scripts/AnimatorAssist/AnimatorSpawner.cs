using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject ObjectToSpawn;

    [SerializeField]
    bool myDirection;

    public void Spawn()
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(ObjectToSpawn);
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
}
