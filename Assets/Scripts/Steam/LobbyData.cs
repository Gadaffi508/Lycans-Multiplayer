using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public int lobbyMemmbers;

    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI lobbyMemmbersText;

    public void SetLobbyData()
    {
        if (lobbyName == "") lobbyNameText.text = "Null";
        else lobbyNameText.text = lobbyName;

        lobbyMemmbersText.text = lobbyMemmbers + " /10";
    }

    public void JoinLobby()
    {
        SteamLobbyManager.Instance.JoinLobby(lobbyID);
    }
}
