using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class ProjectileBullet : NetworkBehaviour
{
    public SphereCollider coll;
    public float DefRadius = 0.5f;
    public float ExRadius = 9f;
    public float Damage = 20f;
    public string OwnerTag;

    private void OnEnable() {
        coll.radius = DefRadius;
    }
    private void OnDisable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerDetectrange") && !other.CompareTag(OwnerTag))
        {
            coll.radius = ExRadius;

            if (other.TryGetComponent<EnemyCore>(out var enemy))
            {
                other.GetComponent<EnemyCore>().TakeDamage(Damage);
            }
            else if (other.TryGetComponent<HpManage>(out var player))
            {
                other.GetComponent<HpManage>().TakeDamageServerRpc(Damage);
            }

            ObjectPoller.ObjPollInstance.SpawnFromPool("ExplosionFx", transform.position, Quaternion.identity);
            Invoke("DelayAble",0.1f);
        }
    }

    void DelayAble(){
        this.gameObject.SetActive(false);
    }
}
