using System.Collections.Generic;
using System.Linq;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobbyController : MonoBehaviour
{
    public static SteamLobbyController Instance;
    private void Awake() => Instance = this;

    [Header("Player Item")] 
    public GameObject playerListItemViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    [Header("Lobby")] 
    public ulong currentLobbyID;
    public bool playerItemCreated = false;

    private List<SteamPlayerLıstItem> _playerListItems = new List<SteamPlayerLıstItem>();
    public SteamPlayerObject localObject;
    
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

    public void HostLobby()
    {
        SteamLobbyManager.Instance.HostLobby();
    }

    public void UpdateLobbyName()
    {
        currentLobbyID = Manager.GetComponent<SteamLobbyManager>().CurrentLobbyID;
    }

    public void UpdatePlayerList()
    {
        CreateHostPlayerItem();
        CreateClientPlayerItem();
        RemovePlayerItem();
        UpdatePlayerItem();
    }

    #region UpdatePlayerFunc

    public void CreateHostPlayerItem()
    {
        if (playerItemCreated) return;

        foreach (SteamPlayerObject player in _manager.GamePlayer)
        {
            if (!_playerListItems.Any(item => item.data.connectionID == player.data.connectionID))
            {
                CreatePlayerItem(player);
            }
        }

        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (SteamPlayerObject player in _manager.GamePlayer)
        {
            if (!_playerListItems.Any(item => item.data.connectionID == player.data.connectionID))
            {
                CreatePlayerItem(player);
            }
        }
    }
    
    public void RemovePlayerItem()
    {
        _playerListItems.RemoveAll(item => !_manager.GamePlayer.Any(player => player.data.connectionID == item.data.connectionID));
    }

    public void UpdatePlayerItem()
    {
        foreach (SteamPlayerLıstItem playerList in _playerListItems)
        {
            SteamPlayerObject player = _manager.GamePlayer.Find(p => p.data.connectionID == playerList.data.connectionID);
            if (player != null)
            {
                playerList.data.playerName = player.playerName;
                playerList.SetPlayerValues();
            }
        }
    }
    
    private void CreatePlayerItem(SteamPlayerObject player)
    {
        GameObject newPlayerItem = Instantiate(playerListItemPrefab, playerListItemViewContent.transform);
        SteamPlayerLıstItem newPlayerItemComponent = newPlayerItem.GetComponent<SteamPlayerLıstItem>();
        newPlayerItemComponent.data.playerName = player.playerName;
        newPlayerItemComponent.data.connectionID = player.data.connectionID;
        newPlayerItemComponent.data.playerSteamID = player.data.playerSteamId;
        newPlayerItemComponent.SetPlayerValues();
        
        _playerListItems.Add(newPlayerItemComponent);
    }

    #endregion
    
    public void FindLocalPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localObject = localPlayerObject.GetComponent<SteamPlayerObject>();
    }

    public void StartGame(string sceneName)
    {
        localObject.CanStartGame(sceneName);
    }
}
