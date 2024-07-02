using UnityEngine;
using UnityEngine.UI;

public class PlayersGetData : MonoBehaviour
{
    public string playerName;
    
    public double playerMS;
    
    [SerializeField] private Text playerNameText,playerMSText;

    public void UpdatePlayerInformation()
    {
        playerNameText.text = playerName;
        playerMSText.text = playerMS + " MS";
    }
}
