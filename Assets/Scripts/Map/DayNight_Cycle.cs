using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class DayNight_Cycle : NetworkBehaviour
{
    public Transform lightDir;

    bool islogin;

    public override void NetworkStart()
    {
        islogin = true;
    }

    private void Update()
    {
        if (islogin)
        {
            if (IsHost)
            {
                RotateSkyServerRpc();
            }

            if (lightDir.rotation.x > 0)
            {
                Debug.Log("3333");
                lightDir.GetComponent<Light>().intensity = 1;
            }
            else
            {
                Debug.Log("4444");
                lightDir.GetComponent<Light>().intensity = 0;
            }
        }
    }

    [ServerRpc]
    void RotateSkyServerRpc()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, 3f * Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }
}
