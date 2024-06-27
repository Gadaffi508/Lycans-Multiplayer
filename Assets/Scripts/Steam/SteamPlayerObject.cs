using Steamworks;
using Mirror;
using UnityEngine;

public class SteamPlayerObject : NetworkBehaviour
{
    [SyncVar] public GameObject playerModel;
    
    [SyncVar] public int connectionID, playerIdNumber;
    [SyncVar] public ulong playerSteamId;

    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string playerName;

    #region Singleton

    private MyNetworkManager _manager;
    private MyNetworkManager Manager
    {
        get
        {
            if (_manager != null) return _manager;
            return _manager = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    #endregion

    public override void OnStartAuthority()
    {
        DontDestroyOnLoad(this);
        SetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        SteamLobbyController.Instance.FindLocalPlayer();
        SteamLobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayer.Add(this);
        SteamLobbyController.Instance.UpdateLobbyName();
        SteamLobbyController.Instance.UpdatePlayerLıst();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayer.Remove(this);
    }

    [Command]
    void SetPlayerName(string _playerName)
    {
        this.PlayerNameUpdate(this.playerName,_playerName);
    }
    
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.playerName = newValue;
        }

        if (isClient)
        {
            SteamLobbyController.Instance.UpdatePlayerLıst();
        }
    }

    public void CanStartGame(string sceneName)
    {
        if (authority) CmdStartGame(sceneName);
    }

    [Command]
    void CmdStartGame(string sceneName)
    {
        _manager.StartGame(sceneName);
        
        playerModel.SetActive(true);
    }
}
