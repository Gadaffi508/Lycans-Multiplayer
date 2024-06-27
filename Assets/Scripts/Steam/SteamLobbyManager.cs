using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobbyManager : MonoBehaviour
{
    public static SteamLobbyManager Instance;
    //callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    protected Callback<LobbyMatchList_t> lobbyList;
    protected Callback<LobbyDataUpdate_t> lobbyData;

    protected Callback<LobbyChatMsg_t> lobbyChatMsg;
    
    public List<CSteamID> lobbyID = new List<CSteamID>();
    
    //manager
    private MyNetworkManager _manager;

    public ulong CurrentLobbyID;

    private void Start()
    {
        if(!SteamManager.Initialized) return;
        if (Instance == null) Instance = this;
        _manager = GetComponent<MyNetworkManager>();
        
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        lobbyList = Callback<LobbyMatchList_t>.Create(MatchLobby);
        lobbyData = Callback<LobbyDataUpdate_t>.Create(GetLobbyData);
        
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) return;
        
        Debug.Log("Lobby created");
        
        _manager.StartHost();

        CSteamID ulSteamID = new CSteamID(callback.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(ulSteamID, "HostAddress",
            SteamUser.GetSteamID().ToString());
        
        SteamMatchmaking.SetLobbyData(ulSteamID, "name",
            SteamFriends.GetPersonaName().ToString());
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Requested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        if(NetworkServer.active) return;

        CSteamID ulSteamID = new CSteamID(callback.m_ulSteamIDLobby);
        
        Debug.Log("Newtork Adress : ------------------" + _manager.networkAddress);
        
        _manager.networkAddress = SteamMatchmaking.GetLobbyData(ulSteamID, "HostAddress");
        _manager.StartClient();
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 10);
    }

    public void LeaveGame(CSteamID lobbyID)
    {
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    public void JoinLobby(CSteamID _lobbyID)
    {
        SteamMatchmaking.JoinLobby(_lobbyID);
    }

    public void FindLobbies()
    {
        if(lobbyID.Count>0) lobbyID.Clear();
        
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    private void MatchLobby(LobbyMatchList_t callback)
    {
        if (FindLobbyManager.Instance.lobbyList.Count>0) FindLobbyManager.Instance.ClearLobby();

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyID.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }
    }

    private void OnLobbyChatMessage(LobbyChatMsg_t callback)
    {
        byte[] data = new byte[4096];

        CSteamID steamIDUser;
        EChatEntryType chatEntryType = EChatEntryType.k_EChatEntryTypeChatMsg;

        SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID,
            out steamIDUser, data, data.Length, out chatEntryType);

        string message = System.Text.Encoding.UTF8.GetString(data);
        
        SteamChatManager.Instance.DisplayChatMessage(SteamFriends.GetFriendPersonaName(steamIDUser),message);
    }

    private void GetLobbyData(LobbyDataUpdate_t callback)
    {
        FindLobbyManager.Instance.DisplayLobby(lobbyID,callback);
    }
}
