using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class PlayerCore : NetworkBehaviour
{
    [Header("Speed Settings")]
    [Range(0, 100)]
    public float WalkSpeed = 21f;
    [Range(0, 100)]
    public float RunSpeed = 28f;
    public float speed;

    [Header("Cam Settings")]

    [Range(0, 1)]
    public float turnThirdSmooth = 0.05f;
    [Range(0, 1)]
    public float turnSholderSmooth = 0.07f;
    public float turnSmoothVel;

    public Transform Cam;
    protected Rigidbody rb;
    protected LockOnSystem LockOnSystem;
    protected PlayerManager PlayerManager;
    public Animator anima;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
        LockOnSystem = GetComponentInChildren<LockOnSystem>();
        PlayerManager = GetComponent<PlayerManager>();
    }

    protected virtual void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        anima.SetBool("isIdle",true);
        speed = WalkSpeed;
    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void Update()
    {
        Movement();
    }
    protected virtual void LateUpdate()
    {

    }

    protected virtual void Movement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anima.SetBool("isRun",true);
            speed = RunSpeed;
        }
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            anima.SetBool("isRun",false);
            speed = WalkSpeed;
        }

        float h = Input.GetAxis("Horizontal") * speed;
        float v = Input.GetAxis("Vertical") * speed;

        Vector3 direction = new Vector3(h, 0f, v).normalized;

        //rb.velocity = new Vector3(Input.GetAxis("Horizontal") * speed,rb.velocity.y,Input.GetAxis("Vertical") * speed);

        if (PlayerManager.IsChangeCam && !LockOnSystem.IsLockOn)
        {
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Cam.eulerAngles.y, ref turnSmoothVel, turnSholderSmooth);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Space)){
            rb.drag = 1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;

            if (!PlayerManager.IsChangeCam && !LockOnSystem.IsLockOn)
            {
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVel, turnThirdSmooth);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            Vector3 MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            rb.velocity = new Vector3(MoveDir.x * speed, rb.velocity.y, MoveDir.z * speed);
        }
        else
        {
            if(rb.velocity.y == 0f){
                rb.drag = 10f;
            }
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        }
    }
}
