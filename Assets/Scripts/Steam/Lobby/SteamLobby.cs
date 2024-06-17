using System;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public ulong currentLobbyID;
    public Text lobbyNameText;
    
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    
    private const string HostAddressKey = "HostAddress";
    private CustomSteamManager _manager;

    void Start()
    {
        if(!SteamManager.Initialized) return;

        _manager = GetComponent<CustomSteamManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnlObbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly,_manager.maxConnections);
    }

    void OnlObbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby Created Succesfully");
        
        _manager.StartHost();

        var ulSteamIDLobby = new CSteamID(callback.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(ulSteamIDLobby,HostAddressKey,SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(ulSteamIDLobby,"name",SteamFriends.GetPersonaName().ToString());
    }

    void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To join Lobby");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = callback.m_ulSteamIDLobby;
        
        var ulSteamIDLobby = new CSteamID(callback.m_ulSteamIDLobby);
        
        lobbyNameText.gameObject.SetActive(true);
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(ulSteamIDLobby,"name");
        
        if(NetworkServer.active) return;

        _manager.networkAddress = SteamMatchmaking.GetLobbyData(ulSteamIDLobby,HostAddressKey);
        
        _manager.StartClient();
    }
}
