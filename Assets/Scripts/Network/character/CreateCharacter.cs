using SimpleJSON;
using System;

public class CreateCharacter : HttpProvider
{
	private const string CREATE_CHARACTER_URL = "/character/create";

	public CreateCharacter(Action<JSONNode> callback) : base(callback){}

	public void Create()
	{
        GetRandomName rndNamer = new GetRandomName(GetRandomNameResponse);
        rndNamer.Get();
	}

    public void GetRandomNameResponse(JSONNode response)
    {
        JSONNode parameters = new JSONClass();
        parameters["name"] = response["data"];

        performRequest(CREATE_CHARACTER_URL, parameters, true);
    }
}
