using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class GunCore : NetworkBehaviour
{
    public PlayerManager playerManager;

    protected void ProjectileShoot(string tag,Vector3 pos,Quaternion rot, float speed){
        GameObject Bullet = ObjectPoller.ObjPollInstance.SpawnFromPool(tag,pos,rot);
        Bullet.GetComponent<Rigidbody>().AddForce(Bullet.transform.forward * speed,ForceMode.Impulse);
    }
}
