using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMeOnLoad : MonoBehaviour {

    public static List<DontDestroyMeOnLoad> Instances;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if(Instances == null)
        {
            Instances = new List<DontDestroyMeOnLoad>();
        }

        Instances.Add(this);
    }

    public static void ClearAll()
    {
        if(Instances == null || Instances.Count == 0)
        {
            return;
        }

        foreach (DontDestroyMeOnLoad instance in Instances)
        {
            Destroy(instance.gameObject);
        }

        Instances.Clear();

    }


}
