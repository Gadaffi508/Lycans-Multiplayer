using System.Collections.Generic;
using System.Linq;
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

    [Header("CloseOBJ")] [SerializeField] private GameObject[] objects;

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
        if (!playerItemCreated) CreateHostPlayerItem();
        if(_playerListItems.Count < Manager.GamePlayer.Count) CreateClientPlayerItem();
        if(_playerListItems.Count > Manager.GamePlayer.Count)RemovePlayerItem();
        if(_playerListItems.Count == Manager.GamePlayer.Count) UpdatePlayerItem();
    }

    #region UpdatePlayerFunc

    public void CreateHostPlayerItem()
    {
        foreach (SteamPlayerObject player in _manager.GamePlayer)
        {
            CreatePlayer(player);
        }

        playerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (SteamPlayerObject player in _manager.GamePlayer)
        {
            if (!_playerListItems.Any(item => item.connectionID == player.connectionID))
            {
                CreatePlayer(player);
            }
        }
    }

    public void RemovePlayerItem()
    {
        _playerListItems.RemoveAll(item =>
            !_manager.GamePlayer.Any(player => player.connectionID == item.connectionID));
    }

    public void UpdatePlayerItem()
    {
        foreach (SteamPlayerObject player in Manager.GamePlayer)
        {
            foreach (SteamPlayerLıstItem playerList in _playerListItems)
            {
                if (playerList.connectionID == player.connectionID)
                {
                    playerList.playerName = player.playerName;
                    playerList.SetPlayerValues();
                }

            }
        }
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

    public void JoinLobby()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    void CreatePlayer(SteamPlayerObject player)
    {
        GameObject newPlayerItem = Instantiate(playerListItemPrefab, playerListItemViewContent.transform);
        SteamPlayerLıstItem newPlayerItemComponent = newPlayerItem.GetComponent<SteamPlayerLıstItem>();
        newPlayerItemComponent.playerName = player.playerName;
        newPlayerItemComponent.connectionID = player.connectionID;
        newPlayerItemComponent.playerSteamID = player.playerSteamId;
        newPlayerItemComponent.SetPlayerValues();

        _playerListItems.Add(newPlayerItemComponent);
    }
}
