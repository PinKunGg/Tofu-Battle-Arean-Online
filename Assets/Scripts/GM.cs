using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class GM : NetworkBehaviour
{
    public static GM gmInstanse;
    public GameObject[] Cam;
    public GameObject WinPanel;
    public Text WinText;
    public GameObject CapABar;
    public GameObject CapBBar;
    public CapturePoint CapPointA, CapPointB;
    public GameObject Player;
    public override void NetworkStart()
    {
        CapABar.SetActive(true);
        CapBBar.SetActive(true);
    }
    
    [SerializeField]
    private NetworkVariableBool isWin = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, false);
    [SerializeField]
    private NetworkVariableString WinTeam = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    private void OnEnable()
    {
        isWin.OnValueChanged += OnIsWinChange;
        WinTeam.OnValueChanged += OnWinTeamChange;
    }
    private void OnDisable()
    {
        isWin.OnValueChanged -= OnIsWinChange;
        WinTeam.OnValueChanged += OnWinTeamChange;
    }
    private void OnIsWinChange(bool oldValue, bool newValue)
    {
        if (!IsClient) { return; }

        if(isWin.Value){
            WinPanel.SetActive(true);
        }
    }
    private void OnWinTeamChange(string oldValue, string newValue)
    {
        if (!IsClient) { return; }

        if(WinTeam.Value != ""){
            WinText.text = "- Team " + WinTeam.Value + " Win -";
        }
    }

    public bool GetIsWin{
        get{
            return isWin.Value;
        }
    }

    [ServerRpc]
    public void SetWinnerServerRpc(){
        isWin.Value = true;
    }
    [ServerRpc]
    public void SetWinTeamServerRpc(string team){
        WinTeam.Value = team;
    }

    public void AllReset(){
        WinPanel.SetActive(false);
        CapABar.SetActive(false);
        CapBBar.SetActive(false);
        CapPointA.ResetPercent();
        CapPointB.ResetPercent();
        isWin.Value = false;
        WinTeam.Value = "";
    }

    private void Awake()
    {
        if (gmInstanse == null)
        {
            gmInstanse = this;
        }
    }

    [ServerRpc]
    public void RespawnServerRpc(){
        Invoke("DealayRespawnClientRpc",2f);
    }

    [ClientRpc]
    void DealayRespawnClientRpc(){
        Player.transform.position = GameObject.Find("SpawnPoint").transform.position;
        Player.transform.rotation = Quaternion.identity;
        Player.SetActive(true);
    }
}
