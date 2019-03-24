using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ServerUtilDatabaseHandler : HttpProvider
{
    public const string SET_DATABASE_URL = "/content/update";
    public const string GET_DATABASE_URL = "/content/get";

    public ServerUtilDatabaseHandler(Action<JSONNode> callback) : base(callback)
    {
    }

    public void UpdateDatabase(JSONNode databaseJson)
    {
        performRequest(SET_DATABASE_URL, databaseJson, true);
    }

    /// <summary>
    /// Will just display the current database in the console log, in JSON.
    /// The database in the server should be equal to our own in CORE.Data.
    /// </summary>
    public void GetCurrentDatabase()
    {
        performRequest(GET_DATABASE_URL, null, true);
    }
}
