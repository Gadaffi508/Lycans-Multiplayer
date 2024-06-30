using Steamworks;
using Mirror;
using UnityEngine;

public class SteamPlayerObject : NetworkBehaviour
{
    [SyncVar] public GameObject playerModel;
    
    [SyncVar] public int connectionID, playerIdNumber;
    [SyncVar] public ulong playerSteamId;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    [SyncVar] public GameObject myCamera;

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
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        SteamLobbyController.Instance.FindLocalPlayer();
        SteamLobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayer.Add(this);
        SteamLobbyController.Instance.UpdateLobbyName();
        SteamLobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayer.Remove(this);
    }

    public void AllCameraOff()
    {
        GameObject[] allCamera = GameObject.FindGameObjectsWithTag("Camera");

        foreach (GameObject camera in allCamera)
        {
            camera.SetActive(false);
        }
        
        myCamera.SetActive(true);
    }

    [Command]
    void CmdSetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }
    
    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (isClient)
        {
            SteamLobbyController.Instance.UpdatePlayerList();
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