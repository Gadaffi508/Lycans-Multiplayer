using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID lobbyID;

    public string lobbyName;
    public string members;

    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI membersText;

    public void SetLobbyData()
    {
        if (lobbyName == "")
            lobbyNameText.text = "null";
        else 
            lobbyNameText.text = "lobbyName";

        membersText.text = members;
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }
}
