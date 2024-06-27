using System.Collections.Generic;
using System.Linq;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobbyController : MonoBehaviour
{
    public static SteamLobbyController Instance;
    private void Awake() => Instance = this;

    [Header("Player Item")] public GameObject playerLıstItemViewContent;
    public GameObject playerLıstItemPrefab;
    public GameObject localPlayerObject;

    [Header("Lobby")] public ulong currentLobbyID;
    public bool playerItemCreated = false;

    private List<SteamPlayerLıstItem> _playerLıstItems = new List<SteamPlayerLıstItem>();
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

    public void UpdatePlayerLıst()
    {
        CreateHostPlayerItem();
        CreateClientPlayerItem();
        RemovePlayerItem();
        UpdatePlayerItem();
    }

    #region UpdatePlayerFunc

    public void CreateHostPlayerItem()
    {
        if(playerItemCreated) return;

        foreach (SteamPlayerObject player in _manager.GamePlayer)
        {
            if (!_playerLıstItems.Any(ıtem => ıtem.connectionID == player.connectionID))
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
            if (!_playerLıstItems.Any(item => item.connectionID == player.connectionID))
            {
                CreatePlayerItem(player);
            }
        }
    }
    
    public void RemovePlayerItem()
    {
        _playerLıstItems.RemoveAll(item => !_manager.GamePlayer.Any(player => player.connectionID == item.connectionID));
    }

    public void UpdatePlayerItem()
    {
        foreach (SteamPlayerLıstItem playerLıst in _playerLıstItems)
        {
            SteamPlayerObject player = _manager.GamePlayer.Find(p => p.connectionID == playerLıst.connectionID);

            foreach (SteamPlayerLıstItem playerLıstıtem in _playerLıstItems)
            {
                if (player != null)
                {
                    playerLıstıtem.playerName = player.playerName;
                    playerLıstıtem.SetPlayerValues();
                }
            }
        }
    }
    
    private void CreatePlayerItem(SteamPlayerObject player)
    {
        GameObject newPlayerItem = Instantiate(playerLıstItemPrefab, playerLıstItemViewContent.transform);
        SteamPlayerLıstItem newPlayerItemst = newPlayerItem.GetComponent<SteamPlayerLıstItem>();
        newPlayerItemst.playerName = player.playerName;
        newPlayerItemst.connectionID = player.connectionID;
        newPlayerItemst.playerSteamID = player.playerSteamId;
        newPlayerItemst.SetPlayerValues();
        
        _playerLıstItems.Add(newPlayerItemst);
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
