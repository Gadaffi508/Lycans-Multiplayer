using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSteamManager : NetworkManager
{
    [Header("Player Prefab")]
    public PlayerObjectController gamePlayerPrefab;

    public List<PlayerObjectController> gamePlayers = new List<PlayerObjectController>();
    
    private NetworkConnectionToClient _conn;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        _conn = conn;

        InstatePlayer();
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
    }
    
    public void InstatePlayer()
    {
        NetworkServer.AddPlayerForConnection(_conn, SetPlayerObject(_conn));
    }

    GameObject SetPlayerObject(NetworkConnectionToClient conn)
    {
        PlayerObjectController gamePlayerInstance = Instantiate(gamePlayerPrefab);
        gamePlayerInstance.connectionID = conn.connectionId;
        gamePlayerInstance.playerName = SteamFriends.GetPersonaName().ToString();
        gamePlayerInstance.playerIdNumber = gamePlayers.Count + 1;
        var lobbyID = (CSteamID)SteamLobby.Instance.currentLobbyID;
        gamePlayerInstance.playerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, gamePlayers.Count);

        return gamePlayerInstance.gameObject;
    }
}
