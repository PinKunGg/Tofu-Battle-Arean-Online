using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BombSpawner : NetworkBehaviour
{
    public GameObject effectPrefab;
    public GameObject bombPrefab;

    void Update()
    {
        if (!IsOwner) { return; }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnEffectServerRpc();
            Instantiate(effectPrefab, transform.position + (transform.forward * -1.5f), transform.rotation);

            SpawnBombServerRpc(transform.position + (transform.forward * -1.5f), transform.rotation);
        }
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    void SpawnBombServerRpc(Vector3 spawnPos, Quaternion spawnRot)
    {
        NetworkObject bomb = Instantiate(bombPrefab, spawnPos, spawnRot).GetComponent<NetworkObject>();
        bomb.SpawnWithOwnership(OwnerClientId);
    }

    [ServerRpc (Delivery = RpcDelivery.Unreliable)]
    void SpawnEffectServerRpc()
    {
        SpawnEffectClientRpc();
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)]
    void SpawnEffectClientRpc()
    {
        if (IsOwner) { return; }
        Instantiate(effectPrefab, transform.position + (transform.forward * -1.5f), transform.rotation);
    }
}
