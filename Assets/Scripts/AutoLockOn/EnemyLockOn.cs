using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class EnemyLockOn : NetworkBehaviour
{
    GameObject lockui;
    public GameObject Player;

    public override void NetworkStart()
    {
        if(Player != null){
            this.gameObject.tag = Player.gameObject.tag;
        }

        if(IsLocalPlayer){
            this.enabled = false;
        }
    }
    private void Start() {
        
    }
    public void TargetLock()
    {
        lockui = ObjectPoller.ObjPollInstance.GetFromPool("ui_LockOn");
        lockui.GetComponentInChildren<ui_LockOnTarget>().target = this.transform;
        lockui.SetActive(true);
    }
    public void UnLockTarget()
    {
        lockui.SetActive(false);
    }
}
