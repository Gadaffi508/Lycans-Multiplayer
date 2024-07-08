using Mirror;
using UnityEngine;

public class SteamPlayerController : NetworkBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float sprintMultiplier;

    private float _X, _Y, currentSpeed;
    private Vector3 _movement;
    private bool isSprinting;
    private int sprinting;

    private Rigidbody _rb;
    private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(!isOwned || !isClient || !isLocalPlayer) return;

        #region Inputs

        _X = Input.GetAxis("Horizontal");
        _Y = Input.GetAxis("Vertical");
        
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        #endregion
        
        currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        sprinting = isSprinting ? 1 : 0;

        Animations();
        
        CmdMove(_X,_Y,currentSpeed);
    }

    [Command]
    void CmdMove(float x, float y,float _currentspeed) => RpcMove(x,y,_currentspeed);

    [ClientRpc]
    void RpcMove(float x, float y,float _currentspeed)
    {
        Vector3 dir = new Vector3(x * _currentspeed * Time.deltaTime,_rb.velocity.y,y * _currentspeed * Time.deltaTime);

        _rb.velocity = dir;
        
        if (dir == Vector3.zero) return;
        
        transform.rotation = Quaternion.Slerp(transform.rotation, LookDirection(dir), rotationSpeed * Time.deltaTime);
    }

    Quaternion LookDirection(Vector3 dir)
    {
        Quaternion lookDir = Quaternion.LookRotation(dir.normalized);
        
        lookDir = Quaternion.Euler(0f, lookDir.eulerAngles.y, lookDir.eulerAngles.z);

        return lookDir;
    }

    void Animations()
    {
        _animator.SetFloat("velocity",_rb.velocity.magnitude);
        
        _animator.SetFloat("Sprint", sprinting);
        
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex("Fight"), 1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex("Fight"), 0);
        }
    }
}
