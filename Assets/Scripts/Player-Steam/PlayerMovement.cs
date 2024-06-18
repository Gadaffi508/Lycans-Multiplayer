using System;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerMovement : NetworkBehaviour
{
    public float speed;

    public GameObject playerModel;

    private Rigidbody _rb;

    private Vector3 _velocity;
    
    private float _x, _y;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        playerModel.SetActive(false);
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name != "Game") return;

        if (playerModel.activeSelf == false)
        {
            SetPosition();
            playerModel.SetActive(true);
        }

        if(authority) Movement();
    }

    void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5,5),1,Random.Range(-5,5));
    }

    void Movement()
    {
        _x = Input.GetAxis("Horizontal");
        _y = Input.GetAxis("Vertical");

        _velocity = new Vector3(_x,0,_y);

        _rb.velocity = _velocity * Time.deltaTime * speed;
    }
}
