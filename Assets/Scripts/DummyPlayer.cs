using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class DummyPlayer : NetworkBehaviour
{
    float speed = 7f;
    public Rigidbody rb;

    private void Start() {
        Debug.LogWarning("Starto!");
        rb = GetComponent<Rigidbody>();
    }

    public override void NetworkStart()
    {
        if(!IsLocalPlayer){
            this.enabled = false;
        }

        this.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }

    private void Update() {
        Movement();
    }

    protected virtual void Movement()
    {
        rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed,rb.velocity.y,Input.GetAxis("Vertical") * speed);
    }
}
