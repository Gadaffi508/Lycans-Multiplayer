using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLeaveGame : MonoBehaviour
{
    public int sceneID;

    public CSteamID lobbyID;
    
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

    void Start()
    {
        lobbyID = (CSteamID)SteamLobbyManager.Instance.CurrentLobbyID;
    }

    public void LeaveGame()
    {
        if(lobbyID != (CSteamID)0)
            SteamLobbyManager.Instance.LeaveGame(lobbyID);
        else
            Debug.Log("Lobby ID : " + lobbyID);
        
        Manager.StopHost();

        Manager.networkAddress = "HostAddress";
        
        Manager.StopClient();

        SceneManager.LoadScene(sceneID);
    }
}
