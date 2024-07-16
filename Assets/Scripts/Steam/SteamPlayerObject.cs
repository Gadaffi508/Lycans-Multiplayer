using Steamworks;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class SteamPlayerObject : NetworkBehaviour
{
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
    [Header("Player OBJ")]
    [SyncVar] public GameObject playerModel;
    
    [Header("Player Connection Values")]
    [SyncVar] public int connectionID;
    [SyncVar] public int playerIdNumber;
    
    [Header("Player Object Values")]
    public ulong playerSteamId;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    public Transform myCamera;

    private SteamPlayerController _controller;

    public int pingInMs;

    private float NetworkPing => (float)NetworkTime.rtt;

    private void Start()
    {
        _controller = GetComponent<SteamPlayerController>();
        _controller.enabled = false;
    }

    public override void OnStartAuthority()
    {
        DontDestroyOnLoad(this);
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
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
        SteamLobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    void CmdSetPlayerName(string _playerName)
    {
        OnPlayerNameChanged(playerName,_playerName);
    }

    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.playerName = newValue;
        }
        if(isClient)
        {
            SteamLobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string sceneName)
    {
        CmdStartGame(sceneName);
    }

    [Command]
    void CmdStartGame(string sceneName)
    {
        _manager.StartGame(sceneName);
        
        if(!isLocalPlayer) return;
        
        GameObject playerInstance = Instantiate(playerModel, transform.position, Quaternion.identity, transform);
        playerInstance.GetComponent<NetworkIdentity>().netId = GetComponent<NetworkIdentity>().netId * 10;

        _controller.enabled = true;
        
        AddCamera();
    }

    void AddCamera()
    {
        myCamera = Camera.main.transform;

        myCamera.AddComponent<CameraController>();
        
        CameraController camera = myCamera.GetComponent<CameraController>();

        camera.lookAt = this.transform;
        camera.distance = 5;
        camera.sensitivity = 100;
        
        myCamera.SetParent(transform);

    }
}