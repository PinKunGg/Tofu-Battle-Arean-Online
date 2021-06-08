using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Particle_Bullet : NetworkBehaviour
{
    public float Damage = 5f;
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public GameObject Player;

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("PlayerDetectrange") && !other.CompareTag(Player.gameObject.tag))
        {
            if (other.TryGetComponent<EnemyCore>(out var enemy))
            {
                other.GetComponent<EnemyCore>().TakeDamage(Damage);
            }
            else if (other.TryGetComponent<HpManage>(out var player))
            {
                other.GetComponent<HpManage>().TakeDamageServerRpc(Damage);
            }

            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 hitPoint = collisionEvents[i].intersection;
                ObjectPoller.ObjPollInstance.SpawnFromPool("BulletHitFx", hitPoint, Quaternion.identity);
            }
        }
    }

    [ServerRpc]
    void SpawnHitFxServerRpc(Vector3 hitPoint)
    {
        SpawnHitFxClientRpc(hitPoint);
    }

    [ClientRpc]
    void SpawnHitFxClientRpc(Vector3 hitPoint)
    {
        ObjectPoller.ObjPollInstance.SpawnFromPool("BulletHitFx", hitPoint, Quaternion.identity);
        Debug.Log(hitPoint);
        Debug.Log(ObjectPoller.ObjPollInstance.GetFromPool("BulletHitFx").gameObject.name);
    }
}
