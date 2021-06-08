using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class AI_Settup : NetworkBehaviour
{
    [SerializeField]
    List<Behaviour> ai_script;
    private void Start()
    {
        foreach (var item in ai_script)
        {
            item.enabled = false;
        }
    }

    public override void NetworkStart()
    {
        if (!IsHost)
        {
            foreach (var item in ai_script)
            {
                item.enabled = false;
            }
        }

        foreach (var item in ai_script)
        {
            item.enabled = true;
        }
    }
}
