using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance { get; private set; }

    [Header("Lobby Setting")] public Text lobbyNameText;

    [Header("Lobby Player Settings")] public Transform playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    public ulong currentLobbyID;
    public bool playerItemCreated = false;

    public PlayerObjectController localPlayerController;

    [Header("Ready Setting")] public Text readyButtonText;

    private List<PlayerLıstItem> _playerListItems = new List<PlayerLıstItem>();

    #region CustomManagerSingleton

    private CustomSteamManager _manager;

    private CustomSteamManager Manager => _manager ??= CustomSteamManager.singleton as CustomSteamManager;

    #endregion

    void Awake() =>
        Instance = this;

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }
    
    public void HostLobby()
    {
        SteamLobby.Instance.HostLobby();
    }

    public void UpdateButton()
    {
        if (localPlayerController.playerReady)
        {
            readyButtonText.text = "Unready";
        }
        else
        {
            readyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool allReady = false;

        foreach (PlayerObjectController player in _manager.gamePlayers)
        {
            if (player.playerReady) allReady = true;
            else
            {
                allReady = false;
                break;
            }
        }
    }

    public void UpdateLobbyName()
    {
        currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
        var lobbyID = new CSteamID(currentLobbyID);
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(lobbyID, "name") + "'s Lobby";
    }

    public void FindLobbyPlayer()
    {
        localPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = localPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated) CreateHostPlayerItem();
        if (_playerListItems.Count < _manager.gamePlayers.Count) CreateClientPlayerItem();
        if (_playerListItems.Count > _manager.gamePlayers.Count) RemovePlayerItem();
        if (_playerListItems.Count == _manager.gamePlayers.Count) UpdatePlayerItem();
    }

    public void StartGame(string sceneName)
    {
        localPlayerController.CanStartGame(sceneName);
    }

    void CreateHostPlayerItem()
    {
        foreach (var player in _manager.gamePlayers)
        {
            var newPlayerItem = Instantiate(playerListItemPrefab);
            var newPlayerItemScript = newPlayerItem.GetComponent<PlayerLıstItem>();

            InitializePlayerItem(player, newPlayerItemScript);

            newPlayerItem.transform.SetParent(playerListViewContent);
            newPlayerItem.transform.localScale = Vector3.one;

            _playerListItems.Add(newPlayerItemScript);
        }

        playerItemCreated = true;
    }

    void CreateClientPlayerItem()
    {
        foreach (var player in _manager.gamePlayers.Where(player =>
                     _playerListItems.All(b => b.connectionID != player.connectionID)))
        {
            var newPlayerItem = Instantiate(playerListItemPrefab);
            var newPlayerItemScript = newPlayerItem.GetComponent<PlayerLıstItem>();

            InitializePlayerItem(player, newPlayerItemScript);

            newPlayerItem.transform.SetParent(playerListViewContent);
            newPlayerItem.transform.localScale = Vector3.one;

            _playerListItems.Add(newPlayerItemScript);
        }
    }

    void RemovePlayerItem()
    {
        var playerListItemToRemove = _playerListItems
            .Where(playerListItem => _manager.gamePlayers.All(b => b.connectionID != playerListItem.connectionID))
            .ToList();

        foreach (var playerListItem in playerListItemToRemove)
        {
            if (playerListItem != null)
            {
                _playerListItems.Remove(playerListItem);
                Destroy(playerListItem.gameObject);
            }
        }
    }

    void UpdatePlayerItem()
    {
        foreach (var player in _manager.gamePlayers)
        {
            var playerListItemScript =
                _playerListItems.FirstOrDefault(item => item.connectionID == player.connectionID);
            if (playerListItemScript != null)
            {
                playerListItemScript.name = player.playerName;
                playerListItemScript.ready = player.playerReady;
                playerListItemScript.SetPlayerValues();

                if (player == localPlayerController)
                {
                    UpdateButton();
                }
            }
        }

        CheckIfAllReady();
    }

    void InitializePlayerItem(PlayerObjectController player, PlayerLıstItem playerListItemScript)
    {
        playerListItemScript.playerName = player.playerName;
        playerListItemScript.connectionID = player.connectionID;
        playerListItemScript.playerSteamID = player.playerSteamID;
        playerListItemScript.ready = player.playerReady;
        playerListItemScript.SetPlayerValues();
    }
}