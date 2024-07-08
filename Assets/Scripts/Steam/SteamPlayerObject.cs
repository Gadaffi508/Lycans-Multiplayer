using System;
using Steamworks;
using Mirror;
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

    [SyncVar] public PlayerData data;

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

    private void Update()
    {
        if (!NetworkClient.isConnected) return;

        pingInMs = Mathf.RoundToInt(NetworkPing * 1000);
    }

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

    [Command]
    void CmdSetPlayerName(string _playerName)
    {
        playerName = _playerName;
    }

    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (isOwned || isClient || isLocalPlayer)
        {
            this.playerName = newValue;
            SteamLobbyController.Instance.UpdatePlayerList();
        }
        else
        {
            
        }
    }

    public void CanStartGame(string sceneName)
    {
        if (authority) CmdStartGame(sceneName);
    }

    [Command]
    void CmdStartGame(string sceneName)
    {
        Instantiate(data.playerModel, transform.position, Quaternion.identity, transform);

        _manager.StartGame(sceneName);

        _controller.enabled = true;
        
        FindCamera();
    }

    void FindCamera()
    {
        myCamera = Camera.main.transform;
        
        myCamera.SetParent(transform);
        myCamera.position = new Vector3(0,1f,-5.2f);

    }
}