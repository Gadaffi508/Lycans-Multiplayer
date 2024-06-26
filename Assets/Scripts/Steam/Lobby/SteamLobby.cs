using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    void Awake() => Instance = this;
    
    public ulong currentLobbyID;
    
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;
    
    protected Callback<LobbyChatMsg_t> lobbyChatMsg;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();
    
    private const string HostAddressKey = "HostAddress";
    private CustomSteamManager _manager;

    void Start()
    {
        if(!SteamManager.Initialized) return;

        _manager = GetComponent<CustomSteamManager>();

        #region Callbacks

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnlObbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        
        lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessage);

        #endregion

    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,_manager.maxConnections);
    }
    
    public void LeaveGame(CSteamID lobbyID)
    {
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if(lobbyIDs.Count > 0) lobbyIDs.Clear();
        
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }
    
    void OnLobbyChatMessage(LobbyChatMsg_t callback)
    {
        byte[] data = new byte[4096];

        CSteamID steamIDUser;
        EChatEntryType chatEntryType = EChatEntryType.k_EChatEntryTypeChatMsg;

        SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID,
            out steamIDUser, data, data.Length, out chatEntryType);

        string message = System.Text.Encoding.UTF8.GetString(data);
        
        SteamChatManager.Instance.DisplayChatMessage(SteamFriends.GetFriendPersonaName(steamIDUser),message);
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
        
        if(NetworkServer.active) return;
        
        var ulSteamIDLobby = new CSteamID(callback.m_ulSteamIDLobby);

        _manager.networkAddress = SteamMatchmaking.GetLobbyData(ulSteamIDLobby,HostAddressKey);
        
        _manager.StartClient();
    }

    void OnGetLobbyList(LobbyMatchList_t result)
    {
        if(LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.Instance.DisplayLobbies(lobbyIDs,result);
    }
}
