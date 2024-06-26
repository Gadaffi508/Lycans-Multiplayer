using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLıstItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;

    public TextMeshProUGUI playerReadyText;
    public bool ready;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private bool _avatarRecevied;

    void Start() =>
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);

    public void ChangeReadyStatus()
    {
        if (ready)
        {
            playerReadyText.text = "Ready";
            playerReadyText.color = Color.green;
        }
        else
        {
            playerReadyText.text = "Unready";
            playerReadyText.color = Color.red;
        }
    }

    public void SetPlayerValues()
    {
        playerNameText.text = playerName;

        ChangeReadyStatus();
        
        if(!_avatarRecevied) GetPlayerIcon();
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if(ImageID == -1) return;
        playerIcon.texture = GetSteamAsTexture(ImageID);
    }

    void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != playerSteamID) return;
        
        playerIcon.texture = GetSteamAsTexture(callback.m_iImage);
    }

    #region CalculateImage

    Texture2D GetSteamAsTexture(int iImage)
    {
        Texture2D _texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (!isValid) return null;

        byte[] image = new byte[width * height * 4];

        isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
        
        if (!isValid) return null;

        _texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
        _texture.LoadRawTextureData(image);
        _texture.Apply();

        _avatarRecevied = true;
        return _texture;
    }

    #endregion
}
