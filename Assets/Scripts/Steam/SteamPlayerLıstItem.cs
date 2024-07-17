using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamPlayerLıstItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;

    public bool _avatarReceived;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;
    private void Start() => ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);

    public void SetPlayerValues()
    {
        playerNameText.text = playerName;
        if(!_avatarReceived) GetPlayerIcon();
    }

    void GetPlayerIcon()
    {
        int ımageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if(ımageID == -1) return;
        if (playerIcon != null) playerIcon.texture = GetSteamAsTexture(ımageID);
    }

    void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerSteamID)
        {
            if (playerIcon != null) playerIcon.texture = GetSteamAsTexture(callback.m_iImage);
        }
        else return;
    }

    #region GetSteamAvatarImage

    private Texture2D GetSteamAsTexture(int ımage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(ımage, out uint width, out uint height);
        if (isValid)
        {
            byte[] _ımage = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(ımage, _ımage, (int)(width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width,(int)height,TextureFormat.RGBA32,false,true);
                texture.LoadRawTextureData(_ımage);
                texture.Apply();
            }
        }

        _avatarReceived = true;
        return texture;
    }

    #endregion

    void OnDisable()
    {
        _avatarReceived = false;
    }
}
