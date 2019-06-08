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
    public Database Data;

    [SerializeField]
    List<GameObject> CommonPrefabs = new List<GameObject>();

    public Dictionary<string, GameObject> PrefabDatabase = new Dictionary<string, GameObject>();

    [SerializeField]
    List<Sprite> CommonSprites = new List<Sprite>();

    public Dictionary<string, Sprite> SpritesDatabase = new Dictionary<string, Sprite>();


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
        AutoLogin();

        GameObject tempPrefab;
        for(int i=0;i<CommonPrefabs.Count;i++)
        {
            tempPrefab = CommonPrefabs[i];
            PrefabDatabase.Add(tempPrefab.name, tempPrefab);
        }

        Sprite tempSprite;
        for (int i = 0; i < CommonSprites.Count; i++)
        {
            tempSprite = CommonSprites[i];
            SpritesDatabase.Add(tempSprite.name, tempSprite);
        }
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

    #region DEBUG

    public void SwitchCharacter(CharacterClass cClass)
    {
        SocketClient.Instance.SendSwitchCharacter(cClass);
    }

    #endregion

    #region Scene Loading

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
            response["character"]["name"].Value,
            Data.GetClass(response["class"].Value));

        SpawnCharacter(character);
    }

    public void SpawnCharacter(CharacterInfo character)
    {
        character.LastPosition = new Vector3(0, 0, -3f);


        GameObject charObj = Instantiate(character.Class.ClassActor);
        MovementController actor = charObj.GetComponent<MovementController>();

        actor.SetInfo(character ,character.isPlayer);

        charObj.transform.position = new Vector3(0, 0, -3f);

        character.CInstance = charObj;

        CurrentRoom.Characters.Add(character);

        if (character.isPlayer)
        {
            InGamePanelUI.Instance.SetInfo(actor);
            ((SceneCamera)CurrentRoom.Settings.GetSceneEntity("Camera")).SetFollowTarget(CurrentCharacter.CInstance);
        }
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
    }

    #endregion

    #region Room Events Handling

    public void UpdateMovement(JSONNode jSONNode, Vector3 pos, float dirX, float dirY)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(jSONNode["id"].Value);

        if (player.CInstance != null)
        {
            CurrentRoom.GetPlayer(jSONNode["id"].Value).CInstance.GetComponent<MovementController>().SetLastPosition(pos, dirX, dirY);
        }
        else
        {
            Debug.LogError("UPDATE MOVEMENT - Player is dead or missing!");
        }
    }

    public void PlayerStartsAbility(string playerID, string abilityKey)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().StartAbility(Data.GetAbility(abilityKey));
        }
        else
        {
            Debug.LogError("START ABILITY - Player is dead or missing!");
        }
    }

    public void ActorHurt(string playerID, int amount)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().Hurt(amount);
        }
        else
        {
            Debug.LogError("HURT - Player is dead or missing!");
        }
    }

    public void ActorHealed(string playerID, int amount)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().Heal(amount);
        }
        else
        {
            Debug.LogError("HEAL - Player is dead or missing!");
        }
    }

    public void ActorBlocked(string playerID)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().Blocked();
        }
        else
        {
            Debug.LogError("BLOCKED - Player is dead or missing!");
        }
    }

    public void ActorDead(string playerID)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().Death();
        }
        else
        {
            Debug.LogError("DEATH - Player is dead or missing!");
        }
    }

    public void ActorRespawn(string playerID, string classKey)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);
        player.Class = Data.GetClass(classKey);

        SpawnCharacter(player);
    }

    public void PlayerBuffAdded(string playerID, string attackerID,  string buffKey)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);
        CharacterInfo byPlayer = CurrentRoom.GetPlayer(attackerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().AddBuff(Data.GetBuff(buffKey), byPlayer);
        }
        else
        {
            Debug.LogError("ADD BUFF - Player is dead or missing!");
        }
    }

    public void PlayerBuffRemoved(string playerID, string buffKey)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);

        if (player.CInstance != null)
        {
            player.CInstance.GetComponent<MovementController>().RemoveBuff(Data.GetBuff(buffKey));
        }
        else
        {
            Debug.LogError("REMOVE BUFF - Player is dead or missing!");
        }
    }

    public void PlayerCooldownProgress(string playerID, string abilityKey, float cooldownProgress)
    {
        CharacterInfo player = CurrentRoom.GetPlayer(playerID);
        
        AbilityStatus abilityStatus = player.CInstance.GetComponent<MovementController>().Status.GetAbilityStatus(Data.GetAbility(abilityKey));
        abilityStatus.UpdateCooldown(cooldownProgress, true);
    }

    #endregion

    #endregion

    #region Util

    public static Vector3 SplineLerp(Vector3 source, Vector3 target, float Height, float t)
    {
        Vector3 ST = new Vector3(source.x, source.y + Height, source.z);
        Vector3 TT = new Vector3(target.x, target.y + Height, target.z);

        Vector3 STTTM = Vector3.Lerp(ST, TT, t);

        Vector3 STM = Vector3.Lerp(source, ST, t);
        Vector3 TTM = Vector3.Lerp(TT, target, t);

        Vector3 SplineST = Vector3.Lerp(STM, STTTM, t);
        Vector3 SplineTM = Vector3.Lerp(STTTM, TTM, t);

        return Vector3.Lerp(SplineST, SplineTM, t);
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
