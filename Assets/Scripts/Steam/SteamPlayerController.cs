using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SteamPlayerController : NetworkBehaviour
{
    public float speed;
    public float rotationSpeed;

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
        
        _animator.SetFloat("speed",_rb.velocity.magnitude);
        
        CmdMove(_X,_Y);
    }

    [Command]
    void CmdMove(float x, float y) => RpcMove(x,y);

    [ClientRpc]
    void RpcMove(float x, float y)
    {
        Vector3 dir = new Vector3(x, 0, y).normalized;

        _rb.velocity = dir * speed * Time.deltaTime;
        
        if (dir == Vector3.zero) return;
        
        Quaternion lookDir = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationSpeed * Time.deltaTime);
    }
}
