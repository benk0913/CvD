using UnityEngine;

public class CharacterInfo
{
    public string ID;
    public string Name;
    public CharacterClass Class;

    public Vector3 LastPosition;

    public bool isPlayer
    {
        get
        {
            return CORE.Instance.CurrentCharacter.ID == this.ID;
        }
    }

    
    /// <summary>
    /// The In Scene Instance of the character (The one that moves around)
    /// </summary>
    public GameObject CInstance;

    public CharacterInfo(string sId, string sName = "Unknown", CharacterClass cClass = null)
    {
        this.ID    = sId;
        this.Name = sName;
        this.Class = cClass;
    }

}
