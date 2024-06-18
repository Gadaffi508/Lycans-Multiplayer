using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager Instance;
    void Awake() => Instance = this;
    
    public GameObject lobbiesMenu,
        lobbyDataItemPrefab,
        lobbyLisContent;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    public void GetListOfLobbies()
    {
        SteamLobby.Instance.GetLobbiesList();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                LobbyDataEntry ıtemData = createdItem.GetComponent<LobbyDataEntry>();

                ıtemData.lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;

                ıtemData.lobbyName = SteamMatchmaking.GetLobbyData(
                    (CSteamID)lobbyIDs[i].m_SteamID, "name");
                
                ıtemData.members = SteamMatchmaking.GetNumLobbyMembers(
                    (CSteamID)lobbyIDs[i].m_SteamID).ToString() + "/ 10";
                
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();
                createdItem.transform.SetParent(lobbyLisContent.transform);
                createdItem.transform.localScale = Vector3.one;
                
                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }
}
