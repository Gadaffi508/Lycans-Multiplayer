using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamPlayerLıstItem : MonoBehaviour
{
    public PlayerLıstItemData data;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;
    private void Start() => ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);

    public void SetPlayerValues()
    {
        playerNameText.text = data.playerName;
        if(!data._avatarReceived) GetPlayerIcon();
    }

    void GetPlayerIcon()
    {
        int ımageID = SteamFriends.GetLargeFriendAvatar((CSteamID)data.playerSteamID);
        if(ımageID == -1) return;
        playerIcon.texture = GetSteamAsTexture(ımageID);
    }

    void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == data.playerSteamID)
        {
            playerIcon.texture = GetSteamAsTexture(callback.m_iImage);
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

        data._avatarReceived = true;
        return texture;
    }

    #endregion

    void OnDisable()
    {
        data._avatarReceived = false;
    }
}
