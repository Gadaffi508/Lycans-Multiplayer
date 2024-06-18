using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance { get; private set; }

    [Header("Lobby Setting")]
    public Text lobbyNameText;

    [Header("Lobby Player Settings")]
    public Transform playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObject;

    public ulong currentLobbyID;
    public bool playerItemCreated = false;

    public PlayerObjectController localPlayerController;

    private List<PlayerLıstItem> _playerListItems = new List<PlayerLıstItem>();

    #region CustomManagerSingleton

    private CustomSteamManager _manager;

    private CustomSteamManager Manager => _manager ??= CustomSteamManager.singleton as CustomSteamManager;

    #endregion

    private void Awake()
    {
        Instance = this;
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
        if (_playerListItems.Count < Manager.gamePlayers.Count) CreateClientPlayerItem();
        if (_playerListItems.Count > Manager.gamePlayers.Count) RemovePlayerItem();
        if (_playerListItems.Count == Manager.gamePlayers.Count) UpdatePlayerItem();
    }

    private void CreateHostPlayerItem()
    {
        foreach (var player in Manager.gamePlayers)
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

    private void CreateClientPlayerItem()
    {
        foreach (var player in Manager.gamePlayers.Where(player => _playerListItems.All(b => b.connectionID != player.connectionID)))
        {
            var newPlayerItem = Instantiate(playerListItemPrefab);
            var newPlayerItemScript = newPlayerItem.GetComponent<PlayerLıstItem>();

            InitializePlayerItem(player, newPlayerItemScript);

            newPlayerItem.transform.SetParent(playerListViewContent);
            newPlayerItem.transform.localScale = Vector3.one;

            _playerListItems.Add(newPlayerItemScript);
        }
    }

    private void RemovePlayerItem()
    {
        var playerListItemToRemove = _playerListItems
            .Where(playerListItem => Manager.gamePlayers.All(b => b.connectionID != playerListItem.connectionID))
            .ToList();

        foreach (var playerListItem in playerListItemToRemove)
        {
            _playerListItems.Remove(playerListItem);
            Destroy(playerListItem.gameObject);
        }
    }

    private void UpdatePlayerItem()
    {
        foreach (var player in Manager.gamePlayers)
        {
            var playerListItemScript = _playerListItems.FirstOrDefault(item => item.connectionID == player.connectionID);
            if (playerListItemScript != null)
            {
                playerListItemScript.name = player.playerName;
                playerListItemScript.SetPlayerValues();
            }
        }
    }

    private void InitializePlayerItem(PlayerObjectController player, PlayerLıstItem playerListItemScript)
    {
        playerListItemScript.playerName = player.playerName;
        playerListItemScript.connectionID = player.connectionID;
        playerListItemScript.playerSteamID = player.playerSteamID;
        playerListItemScript.SetPlayerValues();
    }
}
