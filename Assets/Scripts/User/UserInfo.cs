
using System.Collections.Generic;

public class UserInfo
{
    public string UserID;
    public List<CharacterInfo> Characters = new List<CharacterInfo>();

    public UserInfo(string id)
    {
        this.UserID = id;
    }
}
