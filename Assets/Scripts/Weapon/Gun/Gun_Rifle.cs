using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Gun_Rifle : GunCore
{
    public GameObject BulletSpawnPos;
    public Animator anima;

    bool isFire;
    float FireRate = 0.1f;

    private void OnEnable() {
        BulletSpawnPos.SetActive(true);
    }

    private void OnDisable() {
        BulletSpawnPos.SetActive(false);
    }
    private void Update() {
        if(!IsLocalPlayer){
            return;
        }
        
        if(Input.GetButtonDown("Fire1") && !playerManager.IsPause && !isFire){
            isFire = true;
            ParticleShootServerRpc();
            Invoke("ResetFire",FireRate);
        }
    }
    [ServerRpc]
    void ParticleShootServerRpc(){
        ParticleShootClientRpc();
    }

    [ClientRpc]
    void ParticleShootClientRpc(){
        BulletSpawnPos.GetComponentInChildren<ParticleSystem>().Play();
        anima.SetTrigger("isAttack");
    }

    void ResetFire(){
        isFire = false;
    }
}
