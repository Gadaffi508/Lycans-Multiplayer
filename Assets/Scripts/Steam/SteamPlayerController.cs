using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SteamPlayerController : NetworkBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float sprintMultiplier;

    private float _X, _Y;
    private Vector3 _movement;

    private Rigidbody _rb;
    private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(!isLocalPlayer) return;

        _X = Input.GetAxis("Horizontal");
        _Y = Input.GetAxis("Vertical");
        
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        int sprinting = isSprinting ? 1 : 0;
        
        _animator.SetFloat("velocity",_rb.velocity.magnitude);
        _animator.SetFloat("Sprint", sprinting);
        
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
        
        Quaternion lookDir = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationSpeed * Time.deltaTime);
    }
}
