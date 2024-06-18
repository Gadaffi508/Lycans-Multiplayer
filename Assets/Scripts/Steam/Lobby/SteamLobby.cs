using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    void Awake() => Instance = this;
    
    public ulong currentLobbyID;

    public GameObject lobbyObject;
    
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();
    
    private const string HostAddressKey = "HostAddress";
    private CustomSteamManager _manager;

    void Start()
    {
        if(!SteamManager.Initialized) return;
        
        lobbyObject.SetActive(false);

        _manager = GetComponent<CustomSteamManager>();

        #region Callbacks

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnlObbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);

        #endregion

    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,_manager.maxConnections);
        
        lobbyObject.SetActive(true);
    }

    public void GetLobbiesList()
    {
        if(lobbyIDs.Count > 0) lobbyIDs.Clear();
        
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault);
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
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
        
        if(NetworkServer.active) return;

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
