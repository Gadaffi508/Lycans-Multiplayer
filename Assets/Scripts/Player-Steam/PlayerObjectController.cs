using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerObjectController : NetworkBehaviour
{
    [Header("Player Steam Property")] [SyncVar]
    public int connectionID;

    [SyncVar] public int playerIdNumber;
    [SyncVar] public ulong playerSteamID;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string playerName;
    
    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool playerReady;

    #region CustomManagerSingleton

    private CustomSteamManager _manager;

    private CustomSteamManager Manager
    {
        get
        {
            if (_manager != null) return _manager;
            return _manager = CustomSteamManager.singleton as CustomSteamManager;
        }
    }

    #endregion

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLobbyPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.gamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.gamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    void CmdSetPlayerName(string _playerName)
    {
        this.PlayerNameUpdate(this.playerName, _playerName);
    }
    
    [Command]
    void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.playerReady, !this.playerReady);
    }

    public void ChangeReady()
    {
        if (authority) CmdSetPlayerReady();
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer) this.playerName = newValue;

        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }

    public void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer) this.playerReady = newValue;

        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }
}