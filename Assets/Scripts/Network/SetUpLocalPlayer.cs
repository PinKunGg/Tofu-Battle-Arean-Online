using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine.UI;

public class SetUpLocalPlayer : NetworkBehaviour
{
    public List<Behaviour> PlayerScript;
    public Behaviour LockOnSys;
    public Text nameLable;

    public NetworkVariable<string> playerName = new NetworkVariable<string>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, "Player");

    private void OnEnable()
    {
        playerName.OnValueChanged += OnPlayerNameChanged;
    }

    private void OnDisable()
    {
        playerName.OnValueChanged -= OnPlayerNameChanged;
    }

    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        //if (!IsLocalPlayer) { return; }

        gameObject.name = newValue;
        nameLable.text = newValue;
        Debug.LogFormat("old new : {0} >> new name : {1}", oldValue, newValue);
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;

        if (!IsLocalPlayer)
        {
            LockOnSys.enabled = false;

            foreach (var item in PlayerScript)
            {
                item.enabled = false;
            }

            this.enabled = false;
        }
    }

    public override void NetworkStart()
    {
        if(GameObject.FindObjectsOfType<PlayerCount>().Length % 2 == 0){
            this.gameObject.tag = "PlayerA";
        }
        else
        {
            this.gameObject.tag = "PlayerB";
        }
        
        if (!IsLocalPlayer)
        {
            LockOnSys.enabled = false;

            foreach (var item in PlayerScript)
            {
                item.enabled = false;
            }

            foreach (var item in GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = 14;
            }

            return;
        }

        foreach (var item in GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 13;
        }

        Invoke("DelayLockOnSys",0.5f);
    }

    void DelayLockOnSys(){
        LockOnSys.enabled = true;
    }
}
