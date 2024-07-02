using System;
using Steamworks;
using Mirror;
using UnityEngine;

public class SteamPlayerObject : NetworkBehaviour
{
    [SyncVar] public PlayerData data;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    public Camera myCamera;

    private SteamPlayerController _controller;

    private void Start()
    {
        _controller = GetComponent<SteamPlayerController>();
        _controller.enabled = false;
    }
        

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
            Debug.Log(camera.name);
            camera.SetActive(false);
        }

        Debug.Log("camera name" + myCamera.gameObject.name);
        
        myCamera.gameObject.SetActive(true);
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
        GameObject player = Instantiate(data.playerModel, transform.position, Quaternion.identity,transform);

        myCamera = player.GetComponentInChildren<Camera>();
        
        _manager.StartGame(sceneName);

        _controller.enabled = true;
    }
}