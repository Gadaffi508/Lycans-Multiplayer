using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyData : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public int lobbyMemmbers;

    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI lobbyMemmbersText;

    public Button joinButton;

    public void SetLobbyData()
    {
        if (lobbyName == "") lobbyNameText.text = "Null";
        else lobbyNameText.text = lobbyName;

        lobbyMemmbersText.text = lobbyMemmbers + " /10";
    }

    public void JoinLobby()
    {
        joinButton.interactable = false;
        SteamLobbyController.Instance.JoinLobby();
        SteamLobbyManager.Instance.JoinLobby(lobbyID);
        SteamChatManager.Instance.OpenChat();
    }
}
