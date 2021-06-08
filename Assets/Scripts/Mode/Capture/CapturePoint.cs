using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class CapturePoint : NetworkBehaviour
{
    public Slider CapBar;
    public Text CapPercent;
    bool isCapture;
    public string Team;
    public string TeamCap = "Player";

    [SerializeField]
    private NetworkVariableFloat percent = new NetworkVariableFloat(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 0);

    private void OnEnable()
    {
        percent.OnValueChanged += OnPercentChange;
    }

    private void OnDisable()
    {
        percent.OnValueChanged -= OnPercentChange;
    }

    private void OnPercentChange(float oldValue, float newValue)
    {
        if (!IsClient) { return; }

        if(Mathf.Floor(percent.Value) < 0){
            percent.Value = 0;
        }

        CapBar.value = percent.Value;
        CapPercent.text = (Mathf.Floor(percent.Value)).ToString() + " %";
    }

    private void Start()
    {
        CapBar.maxValue = 100f;
        CapBar.value = 0f;
        CapPercent.text = (Mathf.Floor(percent.Value)).ToString() + " %";
    }

    private void Update()
    {
        if (!isCapture && CapBar.value > 0 && !GM.gmInstanse.GetIsWin)
        {
            DecreasePercentServerRpc();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(TeamCap) && !GM.gmInstanse.GetIsWin)
        {
            isCapture = true;

            if (CapBar.value < CapBar.maxValue)
            {
                IncreasePercentServerRpc();
            }
            else
            {
                GM.gmInstanse.SetWinnerServerRpc();
                GM.gmInstanse.SetWinTeamServerRpc(Team);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TeamCap) && CapBar.value < CapBar.maxValue && !GM.gmInstanse.GetIsWin)
        {
            Invoke("DecreaseCapBar", 1f);
        }
    }

    void DecreaseCapBar()
    {
        isCapture = false;
    }

    [ServerRpc]
    void IncreasePercentServerRpc()
    {
        percent.Value += 1f * Time.deltaTime;
    }

    [ServerRpc]
    void DecreasePercentServerRpc()
    {
        percent.Value -= 1f * Time.deltaTime;
    }
    public void ResetPercent(){
        percent.Value = 0f;
        CapBar.value = 0f;
    }
}
