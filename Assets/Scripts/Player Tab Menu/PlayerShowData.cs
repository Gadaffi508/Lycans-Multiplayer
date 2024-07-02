using System;
using UnityEngine;

public class PlayerShowData : MonoBehaviour
{
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

    public GameObject playerGetData;
    
    public GameObject playersTabMenu;

    public Transform viewContent;

    private void Start()
    {
        for (int i = 0; i < Manager.GamePlayer.Count; i++)
        {
            GameObject player = Instantiate(playerGetData,viewContent);
            PlayersGetData playerData = player.GetComponent<PlayersGetData>();

            playerData.playerName = Manager.GamePlayer[i].playerName;
            playerData.UpdatePlayerInformation();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            playersTabMenu.SetActive(true);
        else if(Input.GetKeyUp(KeyCode.Tab))
            playersTabMenu.SetActive(false);
    }
}
