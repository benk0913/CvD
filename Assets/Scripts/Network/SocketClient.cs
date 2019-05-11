using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using SimpleJSON;
using System;

public class SocketClient : MonoBehaviour
{
    private const int BITCH_WAIT_FRAMES = 5;

    #region Config
    public bool DebugMode = false;

    #endregion

    #region Essential

    protected WebSocketConnector webSocketConnector;

    protected Socket CurrentSocket;

    public static SocketClient Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        webSocketConnector = new WebSocketConnector();
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }


    #endregion

    #region Public Methods

    public void ConnectToGame(string charID)
    {
        BroadcastEvent("Connecting to server..");
        CurrentSocket = webSocketConnector.connect(charID); //UserID

        CurrentSocket.On("connect", OnConnect);
        CurrentSocket.On("disconnect", OnDisconnect);
        CurrentSocket.On("error", OnError);

        CurrentSocket.On("event_error", OnEventError);

        CurrentSocket.On("actor_join_room", OnActorJoinRoom);
        CurrentSocket.On("actor_leave_room", OnActorLeaveRoom);
        CurrentSocket.On("actor_move_room", OnMoveRoom);

        CurrentSocket.On("movement", OnMovement);

        CurrentSocket.On("cooldown_progress", OnCooldownProgress);

        CurrentSocket.On("player_hurt", OnTakeDamage);
        CurrentSocket.On("player_heal", OnTakeHeal);
        CurrentSocket.On("player_block", OnTakeBlock);
        CurrentSocket.On("player_ded", OnPlayerDead);
        CurrentSocket.On("player_respawn", OnPlayerRespawn);

        CurrentSocket.On("player_use_ability", OnPlayerUseAbility);
        
        CurrentSocket.On("buff_activated", OnBuffAdded);
        CurrentSocket.On("buff_cleared", OnBuffRemoved);
    }

    public void Disconnect()
    {
        if (CurrentSocket != null)
        {
            CurrentSocket.Disconnect();
            CurrentSocket.Off();
        }
    }

    #endregion

    #region Callbacks

    private void OnError(Socket socket, Packet packet, object[] args)
    {
        Error error = args[0] as Error;
        BroadcastEvent("On error: " + error);
    }
    
    private void OnEventError(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("On event error: " + data.AsObject.ToString());
    }

    private void OnDisconnect(Socket socket, Packet packet, object[] args)
    {
        BroadcastEvent("On disconnect");
    }

    protected void OnConnect(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("On connect");
        CORE.Instance.OnConnect();
    }

    protected void OnActorJoinRoom(Socket socket, Packet packet, params object[] args)
    {

        JSONNode characterData = (JSONNode)args[0];
        
        CORE.Instance.SpawnCharacter(characterData);

        BroadcastEvent("Actor has joined the room "+characterData.ToString());
    }

    protected void OnActorLeaveRoom(Socket socket, Packet packet, params object[] args)
    {
        BroadcastEvent("Actor has left the room");

        //JSONNode data = (JSONNode)args[0];
        //Game.Instance.RemoveOtherPlayerCharacter(data["id"]);
    }

    protected void OnMovement(Socket socket, Packet packet, params object[] args)
    {

        JSONNode data = (JSONNode)args[0];

        CORE.Instance.UpdateMovement(data, new Vector3(data["x"].AsFloat, data["y"].AsFloat, data["z"].AsFloat), data["angle"].AsFloat, data["velocity"].AsFloat);
    }


    protected void OnMoveRoom(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Moved Room - " + data.ToString());
        

        CORE.Instance.LoadScene(data["room"], data["character"]);
    }

    protected void OnTakeDamage(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Damage taken | " + data.ToString());

        CORE.Instance.ActorHurt(data["player_id"].Value, data["damage"].AsInt);
    }

    protected void OnTakeHeal(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Heals taken | " + data.ToString());

        CORE.Instance.ActorHealed(data["player_id"].Value, data["heal"].AsInt);
    }

    protected void OnTakeBlock(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Blocked Attack | " + data.ToString());

        CORE.Instance.ActorBlocked(data["player_id"].Value);
    }

    protected void OnPlayerRespawn(Socket socket, Packet packet, object[] args)
    {

        JSONNode data = (JSONNode)args[0];

        CORE.Instance.ActorRespawn(data["player_id"].Value, data["class_key"].Value);

        BroadcastEvent("Actor Has Been Resurrected "+data.ToString());
    }

    protected void OnPlayerUseAbility(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent(data["player_id"].Value + " Preforms Ability " + data["ability_key"].Value);

        CORE.Instance.PlayerStartsAbility(data["player_id"].Value, data["ability_key"]);
    }

    protected void OnPlayerDead(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];
        BroadcastEvent("Actor "+ data["player_id"].Value+" Has Died");

        CORE.Instance.ActorDead(data["player_id"].Value);
    }

    private void OnBuffAdded(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Buff Added "+data.ToString());

        CORE.Instance.PlayerBuffAdded(data["player_id"].Value, data["attacker_id"].Value ,data["buff_key"].Value);

    }
    
    private void OnBuffRemoved(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Buff Removed " + data.ToString());

        CORE.Instance.PlayerBuffRemoved(data["player_id"].Value, data["buff_key"].Value);

    }

    private void OnCooldownProgress(Socket socket, Packet packet, object[] args)
    {
        JSONNode data = (JSONNode)args[0];

        BroadcastEvent("Cooldown Progress " + data.ToString());


        CORE.Instance.PlayerCooldownProgress(data["player_id"], data["ability_key"].Value, data["total_progress"].AsFloat);

    }

    #endregion

    #region Emittions

    public void EmitLoadedScene()
    {
        BroadcastEvent("Emitted : LoadedScene");
        JSONNode node = new JSONClass();

        CurrentSocket.Emit("entered_room", node);
    }

    public void EmitEnteredPortal(string portal)
    {
        BroadcastEvent("Emitted : Entering Portal");
        JSONNode node = new JSONClass();

        node["portal"] = portal;

        CurrentSocket.Emit("entered_portal", node);
    }

    public void EmitMovement(Vector3 pos, float direction, float velocity)
    {
        JSONNode node = new JSONClass();

        node["x"].AsFloat = pos.x;
        node["y"].AsFloat = pos.y;
        node["z"].AsFloat = pos.z;
        node["angle"].AsFloat = direction;
        node["velocity"].AsFloat = velocity;

        CurrentSocket.Emit("movement", node);
    }

    public void SendUsedAbility(string abilityKey)
    {
        JSONNode node = new JSONClass();

        node["ability_key"] = abilityKey;

        CurrentSocket.Emit("used_ability", node);

        BroadcastEvent("SENT - Used Ability " + node.ToString());
    }

    public void SendHitAbility(List<string> targetIDs, string abilityKey)
    {
        JSONNode node = new JSONClass();

        node["ability_key"] = abilityKey;

        for (int i = 0; i < targetIDs.Count; i++)
        {
            node["target_ids"][i] = targetIDs[i];
        }

        CurrentSocket.Emit("hit_ability", node);

        BroadcastEvent("SENT - Hit Ability " + node.ToString());
    }

    public void SendSwitchCharacter(CharacterClass cClass)
    {
        JSONNode node = new JSONClass();

        node["class_key"] = cClass.name;
        
        CurrentSocket.Emit("switch_character", node);

        BroadcastEvent("SENT - Switch Character " + node.ToString());
    }

    #endregion

    #region Internal


    protected void BroadcastEvent(string info)
    {
        if (DebugMode)
        {
            Debug.Log(this + " | " + info);
        }
    }


    #endregion

}
