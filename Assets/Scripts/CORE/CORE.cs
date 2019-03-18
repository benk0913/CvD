using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CORE : MonoBehaviour {

    #region Parameters

    public static CORE Instance;

    public SceneInfo CurrentRoom;
    public UserInfo CurrentUser;
    public CharacterInfo CurrentCharacter;

    //TODO REMOVE ME LATER! AFTER RANDOM REGISTER IS COMPLETE.
    string tempRandomUsername;

    [SerializeField]
    public InputMap Input;

    [SerializeField]
    public Database Data = new Database();

    #endregion

    #region Initialize

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        Input.Initialize();
        AutoLogin();
    }

    private void AutoLogin()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;

        RegisterHandler rHandler = new RegisterHandler(OnRegistered);


        tempRandomUsername = UnityEngine.Random.Range(0,10000000).ToString()+"a";
        rHandler.Register(tempRandomUsername, tempRandomUsername);

        Debug.Log("Initializing");
    }

    private void OnRegistered(JSONNode response)
    {
        Debug.Log("OnRegister - " + response.ToString());
        LoginHandler lHandler = new LoginHandler(OnLogin);
        lHandler.Login(tempRandomUsername, tempRandomUsername);
    }

    private void OnLogin(JSONNode response)
    {
        Debug.Log("OnLogin - " + response.ToString());

        CurrentUser = new UserInfo(response["_id"].Value);

        for(int i=0;i<response["characters"].Count;i++)
        {
            CurrentUser.Characters.Add(new CharacterInfo(
                response["characters"][i]["_id"].Value,
                response["characters"][i]["name"].Value,
                Data.GetClass(response["characters"][i]["class"].Value)));
        }

        CreateCharacter Character = new CreateCharacter(OnCreatedCharacter);
        Character.Create();
    }

    private void OnCreatedCharacter(JSONNode response)
    {
        Debug.Log("OnCreateCharacter - " + response.ToString());

        SelectCharacter(response["data"][0]["_id"]);
    }

    private void SelectCharacter(string charID)
    {

        CurrentCharacter = new CharacterInfo(
                charID,
                "whatever",
                null);

        SocketClient.Instance.ConnectToGame(CurrentCharacter.ID);
    }

    internal void OnConnect()
    {

    }

    #endregion


    #region Public Methods

    public void LoadScene(JSONNode roomData, JSONNode charData)
    {
        SceneManager.LoadScene(roomData.Value, LoadSceneMode.Single);

        CurrentCharacter = new CharacterInfo(
             charData["_id"].Value,
             charData["name"].Value,
             Data.GetClass(charData["class"].Value));

        CurrentRoom = new SceneInfo();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        if(CurrentRoom != null)
        {
            //TODO Dispose last room here.
        }

        CurrentRoom.SceneReference = newScene;

        StartCoroutine(InitializeSceneContent());

        SocketClient.Instance.EmitLoadedScene();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SpawnCharacter(JSONNode response)
    {
        CharacterInfo character = new CharacterInfo(
            response["character"]["_id"].Value,
            response["character"]["name"].Value);

        SpawnCharacter(character);
    }

    public void SpawnCharacter(CharacterInfo character)
    {
        character.LastPosition = new Vector3(0, 0, -3f);


        GameObject charObj = Instantiate(ResourcesLoader.Instance.GetObject("Actor_Test"));
        charObj.GetComponent<MovementController>().isPlayer = (character.ID == CurrentCharacter.ID);
        charObj.transform.position = new Vector3(0, 0, -3f);

        character.CInstance = charObj;

        CurrentRoom.Characters.Add(character);
    }

    IEnumerator InitializeSceneContent()
    {
        while(CurrentRoom.Settings == null)
        {
            yield return 0;
        }

        while (CurrentRoom.Settings.GetSceneEntity("Camera") == null)
        {
            yield return 0;
        }

        SpawnCharacter(CurrentCharacter);

        ((SceneCamera)CurrentRoom.Settings.GetSceneEntity("Camera")).SetFollowTarget(CurrentCharacter.CInstance);
    }

    public void UpdateMovement(JSONNode jSONNode, Vector3 pos, float dirX, float dirY)
    {
        CurrentRoom.GetPlayer(jSONNode["id"].Value).CInstance.GetComponent<MovementController>().SetLastPosition(pos, dirX, dirY);
    }

    public void PerformAttack(string id)
    {
        CurrentRoom.GetPlayer(id).CInstance.GetComponent<MovementController>().PreformAttack();
    }

    public void ActorHurt(string id)
    {
        CurrentRoom.GetPlayer(id).CInstance.GetComponent<MovementController>().Hurt();
    }

    #endregion

}

public class MiniChar
{
    public string ID;
    public string Name;
    public Vector3 LastPosition;
    public GameObject CharObj;
}
