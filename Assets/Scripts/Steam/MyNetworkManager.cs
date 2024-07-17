using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public SteamPlayerObject gamePlayerPrefabs;
    public List<SteamPlayerObject> GamePlayer = new List<SteamPlayerObject>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        SteamPlayerObject gamePlayerInstance = Instantiate(gamePlayerPrefabs);
        
        gamePlayerInstance.connectionID = conn.connectionId;
        gamePlayerInstance.playerIdNumber = GamePlayer.Count + 1;

        CSteamID _currentLobbID = (CSteamID)SteamLobbyManager.Instance.CurrentLobbyID;
        gamePlayerInstance.playerSteamId =
            (ulong)SteamMatchmaking.GetLobbyMemberByIndex(_currentLobbID, GamePlayer.Count);
        
        NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
