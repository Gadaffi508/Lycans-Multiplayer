using UnityEngine;

[CreateAssetMenu(fileName = "Player Lıst Item Data", menuName = "ScriptableObjects/Player Lıst Item Data")]
public class PlayerLıstItemData : ScriptableObject
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;

    public bool _avatarReceived;
}
