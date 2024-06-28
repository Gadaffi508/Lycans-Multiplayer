using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public Transform cameraHolder;
    public Vector3 offset;

    public override void OnStartAuthority()
    {
        cameraHolder.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!authority) return; 

        CmdMove(transform.position);
    }

    [Command]
    void CmdMove(Vector3 position)
    {
        RpcFollow(position);
    }

    [ClientRpc]
    void RpcFollow(Vector3 position)
    {
        if (cameraHolder != null)
        {
            cameraHolder.position = position + offset;
        }
        else
        {
            Debug.LogError("cameraHolder is null!");
        }
    }
}