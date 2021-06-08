using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class ui_RotateToPlayer : NetworkBehaviour
{
    Transform _player;

    public override void NetworkStart()
    {
        foreach (var item in GameObject.FindObjectsOfType<SetUpLocalPlayer>())
        {
            if (item.enabled)
            {
                _player = item.transform;
                break;
            }
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            this.transform.LookAt(new Vector3(_player.position.x, this.transform.position.y, _player.position.z));
        }
        else
        {
            foreach (var item in GameObject.FindObjectsOfType<SetUpLocalPlayer>())
            {
                if (item.enabled)
                {
                    _player = item.transform;
                    break;
                }
            }
        }
    }
}
