using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class GroundMode : PlayerCore
{
    [Header("Hover Speed Settings")]
    [Range(0, 100)]
    public float H_WalkSpeed = 35f;
    [Range(0, 100)]
    public float H_RunSpeed = 42f;
    float def_WalkSpeed, def_RunSpeed;
    public GameObject HoverFx1, HoverFx2, HoverFxR1, HoverFxR2, HoverFxL1, HoverFxL2;
    ParticleSystem _hoverParFx1, _hoverParFx2, _hoverParFxR1, _hoverParFxR2, _hoverParFxL1, _hoverParFxL2;
    [Header("Jump settings")]
    public float maxJump = 2f;
    public float jumpCount;
    public float jumpForce = 14f;
    public LayerMask groundLayer;
    public Collider PlayerCollider;
    public Transform _hoverDetectLength;

    RaycastHit hit;

    float pressTime;
    bool isHover;
    public bool IsCanHover = true;
    protected override void Start()
    {
        _hoverParFx1 = HoverFx1.GetComponent<ParticleSystem>();
        _hoverParFx2 = HoverFx2.GetComponent<ParticleSystem>();
        _hoverParFxR1 = HoverFxR1.GetComponent<ParticleSystem>();
        _hoverParFxR2 = HoverFxR2.GetComponent<ParticleSystem>();
        _hoverParFxL1 = HoverFxL1.GetComponent<ParticleSystem>();
        _hoverParFxL2 = HoverFxL2.GetComponent<ParticleSystem>();

        if (!IsLocalPlayer)
        {
            return;
        }

        base.Start();

        jumpCount = maxJump;
    }
    protected override void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        base.Update();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            Jump();
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && IsCanHover)
        {
            WalkAnimaServerRpc();

            pressTime += 1f * Time.deltaTime;

            if (pressTime >= 1)
            {
                if (Physics.CheckCapsule(PlayerCollider.bounds.center, new Vector3(PlayerCollider.bounds.center.x, PlayerCollider.bounds.min.y - 2f, PlayerCollider.bounds.center.z), 0.1f, groundLayer))
                {
                    rb.AddForce(Vector3.up * 10f, ForceMode.Acceleration);
                }

                if (!isHover)
                {
                    def_WalkSpeed = WalkSpeed;
                    def_RunSpeed = RunSpeed;
                    ActiveHoverServerRpc();
                    isHover = true;
                }
            }
        }
        else
        {
            IdleAnimaServerRpc();

            pressTime = 0f;
            if (isHover)
            {
                DeActiveHoverServerRpc();
                isHover = false;
            }
        }
    }

    protected override void LateUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        base.LateUpdate();

        if (isGrounded() && !Input.GetKey(KeyCode.Space))
        {
            jumpCount = maxJump;
        }
    }
    [ServerRpc]
    public void ActiveHoverServerRpc()
    {
        ActiveHoverModeClientRpc();
    }

    [ClientRpc]
    void ActiveHoverModeClientRpc()
    {
        var Fx1Main = _hoverParFx1.main;
        var Fx2Main = _hoverParFx2.main;
        var RFx1Main = _hoverParFxR1.main;
        var RFx2Main = _hoverParFxR2.main;
        var LFx1Main = _hoverParFxL1.main;
        var LFx2Main = _hoverParFxL2.main;
        _hoverParFx1.Play();
        Fx1Main.loop = true;
        _hoverParFx2.Play();
        Fx2Main.loop = true;
        _hoverParFxR1.Play();
        RFx1Main.loop = true;
        _hoverParFxR2.Play();
        RFx2Main.loop = true;
        _hoverParFxL1.Play();
        LFx1Main.loop = true;
        _hoverParFxL2.Play();
        LFx2Main.loop = true;
        WalkSpeed = H_WalkSpeed;
        RunSpeed = H_RunSpeed;
    }
    
    [ServerRpc]
    public void DeActiveHoverServerRpc()
    {
        DeActiveHoverModeClientRpc();
    }

    [ClientRpc]
    void DeActiveHoverModeClientRpc()
    {
        var Fx1Main = _hoverParFx1.main;
        var Fx2Main = _hoverParFx2.main;
        var RFx1Main = _hoverParFxR1.main;
        var RFx2Main = _hoverParFxR2.main;
        var LFx1Main = _hoverParFxL1.main;
        var LFx2Main = _hoverParFxL2.main;
        Fx1Main.loop = false;
        Fx2Main.loop = false;
        RFx1Main.loop = false;
        RFx2Main.loop = false;
        LFx1Main.loop = false;
        LFx2Main.loop = false;
        WalkSpeed = def_WalkSpeed;
        RunSpeed = def_RunSpeed;
    }

    
    
    [ServerRpc]
    public void WalkAnimaServerRpc()
    {
        WalkAnimaClientRpc();
    }
    [ClientRpc]
    public void WalkAnimaClientRpc()
    {
        anima.SetBool("isIdle", false);
        anima.SetBool("isWalk", true);
    }
    [ServerRpc]
    public void IdleAnimaServerRpc()
    {
        IdleAnimaClientRpc();
    }
    [ClientRpc]
    public void IdleAnimaClientRpc()
    {
        anima.SetBool("isIdle", true);
        anima.SetBool("isWalk", false);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpCount--;
    }
    public bool isGrounded()
    {
        return Physics.CheckCapsule(PlayerCollider.bounds.center, new Vector3(PlayerCollider.bounds.center.x, PlayerCollider.bounds.min.y, PlayerCollider.bounds.center.z), 0.1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(PlayerCollider.bounds.center.x, PlayerCollider.bounds.min.y, PlayerCollider.bounds.center.z), 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(PlayerCollider.bounds.center.x, PlayerCollider.bounds.min.y - 2f, PlayerCollider.bounds.center.z), 0.1f);
    }
}
