using UnityEngine;

public class CharacterInfo
{
    public string ID;
    public string Name;
    public string Race;
    public string Class;

    public Vector3 LastPosition;
    
    /// <summary>
    /// The In Scene Instance of the character (The one that moves around)
    /// </summary>
    public GameObject CInstance;

    public CharacterInfo(string sId, string sName = "Unknown", string sRace = "Unknown", string sClass = "Unknown")
    {
        this.ID    = sId;
        this.Name = sName;
        this.Race  = sRace;
        this.Class = sClass;
    }

}
