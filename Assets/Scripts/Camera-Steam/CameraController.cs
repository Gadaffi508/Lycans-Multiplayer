using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;

    public Vector3 offset;

    public override void OnStartAuthority()
    {
        cameraHolder.SetActive(true);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != "Game") return;
        
        cameraHolder.transform.position = transform.position + offset;
    }
}
