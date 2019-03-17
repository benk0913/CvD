using SimpleJSON;
using System;

public class GetRandomName : HttpProvider
{
	private const string GET_RANDOM_NAME_URL = "/character/random-name";

	public GetRandomName(Action<JSONNode> callback) : base(callback){}

	public void Get()
	{
		string urlSuffix = GET_RANDOM_NAME_URL + "?g=1";

        performRequest(urlSuffix, new JSONClass(), false);
	}

}
