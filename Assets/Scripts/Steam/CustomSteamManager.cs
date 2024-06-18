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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;

        NetworkServer.AddPlayerForConnection(conn, SetPlayerObject(conn));
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
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
