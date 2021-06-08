using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class PlayerCam : NetworkBehaviour
{
    public Camera Cam;
    public GameObject ThirdCam, SholderCam;
    public override void NetworkStart()
    {
        if (IsLocalPlayer)
        {
            Cam.gameObject.SetActive(true);
            ThirdCam.gameObject.SetActive(true);
            SholderCam.gameObject.SetActive(true);
        }
    }
}
