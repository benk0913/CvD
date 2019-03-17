using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    private void Awake()
    {
        CORE.Instance.CurrentRoom.Settings = this;
    }

    public Dictionary<string, List<Component>> SceneEntities = new Dictionary<string, List<Component>>();

    /// <summary>
    /// Register as "Scene Entity".
    /// 
    /// * NOTE that this component gets referenced in "CORE.Instance.CurrentRoom.Settings" in the first "Awake" of the scene and NOT BEFORE SO.
    /// 
    /// </summary>
    /// <param name="entity"> The component to be registered </param>
    /// <param name="tag"> The tag which contains all objects of such type. </param>
    public void RegisterEntity(Component entity, string tag = "Default")
    {
        if(SceneEntities.ContainsKey(tag))
        {
            if(SceneEntities[tag].Contains(entity))
            {
                Debug.LogError("Scene Entity " + entity + " is already registered!");
            }
            else
            {
                SceneEntities[tag].Add(entity);
            }
        }
        else
        {
            List<Component> tempComponents = new List<Component>();

            tempComponents.Add(entity);

            SceneEntities.Add(tag, tempComponents);
        }
    }

    public Component GetSceneEntity(string tag)
    {
        if(SceneEntities.ContainsKey(tag) && SceneEntities[tag].Count > 0 && SceneEntities[tag][0] != null)
        {
            return SceneEntities[tag][0];
        }

        return null;
    }
}
