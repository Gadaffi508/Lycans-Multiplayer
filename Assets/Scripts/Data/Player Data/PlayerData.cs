using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "ScriptableObjects/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player OBJ")]
    public GameObject playerModel;
    
    [Header("Player Connection Values")]
    public int connectionID;
    public int playerIdNumber;
    
    public ulong playerSteamId;
}
