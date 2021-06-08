using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using Cinemachine;

public class PlayerManager : NetworkBehaviour
{
    [Header("Mode Settings")]
    public bool IsChangeMode;
    public Behaviour[] Mode = new Behaviour[2];
    Rigidbody Rb;

    [Header("Cam Settings")]
    public bool IsChangeCam;
    public CinemachineFreeLook ThirdCam;
    public CinemachineFreeLook SholderCam;
    public Transform LookAt;
    public Transform Follow;

    [Header("Script Settings")]
    public List<Behaviour> PlayerScrpits;
    public int[] PlayerScriptsCheck;

    [Header("Other Settings")]
    public bool IsPause;
    public Text PlayerName;

    LockOnSystem lockOnSystem;

    private void Awake() {
        Mode[0] = GetComponent<GroundMode>();
        Mode[1] = GetComponent<AerialMode>();
        Rb = GetComponent<Rigidbody>();
        lockOnSystem = GetComponentInChildren<LockOnSystem>();
    }

    public override void NetworkStart()
    {
        if(!IsLocalPlayer){
            this.enabled = false;
        }

        if (!IsChangeMode)
        {
            Mode[0].enabled = true;
            Mode[1].enabled = false;
        }
        else
        {
            Mode[0].enabled = false;
            Mode[1].enabled = true;
        }

        PlayerScriptsCheck = new int[PlayerScrpits.Count];
        CheckActiveScripts();

        ThirdCam.Follow = Follow;
        ThirdCam.LookAt = LookAt;
        SholderCam.Follow = Follow;
        SholderCam.LookAt = LookAt;
    }
    private void Start() {
        if(!IsLocalPlayer){
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if(IsPause){
            Rb.velocity = new Vector3(0f, Rb.velocity.y, 0f);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !lockOnSystem.IsLockOn && !IsPause)
        {
            ChangeCam();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPause = !IsPause;

            if (IsPause)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                DisableAllScript();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                EnableAllScript();
            }
        }
    }
    private void LateUpdate()
    {
        
    }

    void ChangeCam()
    {
        IsChangeCam = !IsChangeCam;
        //True = SholderCam
        //False = ThirdCam

        if (IsChangeCam)
        {
            PlayerName.gameObject.SetActive(false);
            ThirdCam.Priority = 10;
            SholderCam.m_YAxis = ThirdCam.m_YAxis;
            SholderCam.m_XAxis = ThirdCam.m_XAxis;
            SholderCam.Priority = 11;
        }
        else
        {
            PlayerName.gameObject.SetActive(true);
            ThirdCam.m_YAxis = SholderCam.m_YAxis;
            ThirdCam.m_XAxis = SholderCam.m_XAxis;
            ThirdCam.Priority = 11;
            SholderCam.Priority = 10;
        }
    }

    void CheckActiveScripts()
    {

        for (int i = 0; i < PlayerScrpits.Count; i++)
        {
            if (PlayerScrpits[i].enabled)
            {
                PlayerScriptsCheck[i] = 1;
            }
            else
            {
                PlayerScriptsCheck[i] = 0;
            }
        }
    }

    public void DisableAllScript()
    {
        CheckActiveScripts();

        foreach (var script in PlayerScrpits)
        {
            script.enabled = false;
        }

        ThirdCam.m_YAxis.m_MaxSpeed = 0;
        ThirdCam.m_XAxis.m_MaxSpeed = 0;
        SholderCam.m_YAxis.m_MaxSpeed = 0;
        SholderCam.m_XAxis.m_MaxSpeed = 0;

    }

    public void EnableAllScript()
    {
        for (int i = 0; i < PlayerScrpits.Count; i++)
        {
            if (PlayerScriptsCheck[i] == 1)
            {
                PlayerScrpits[i].enabled = true;
            }
            else
            {
                PlayerScrpits[i].enabled = false;
            }
        }

        ThirdCam.m_YAxis.m_MaxSpeed = 2;
        ThirdCam.m_XAxis.m_MaxSpeed = 250;
        SholderCam.m_YAxis.m_MaxSpeed = 2;
        SholderCam.m_XAxis.m_MaxSpeed = 250;
    }
}
