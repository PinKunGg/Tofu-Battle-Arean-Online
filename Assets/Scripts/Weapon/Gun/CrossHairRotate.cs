using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class CrossHairRotate : NetworkBehaviour
{
    Ray ray;
    RaycastHit hit;
    public Camera Camera;
    public Image UI_Crosshair;
    public LockOnSystem LockOnSystem;
    public PlayerManager PlayerManager;
    float turnSmoothVel;

    public override void NetworkStart()
    {
        if (!IsLocalPlayer)
        {
            this.enabled = false;
        }
    }

    private void Start()
    {
        UI_Crosshair = GameObject.Find("ui_CrossHair").GetComponent<Image>();
    }

    private void Update()
    {
        if (!LockOnSystem.IsLockOn)
        {
            if (PlayerManager.IsChangeCam)
            {
                UI_Crosshair.enabled = true;

                ray = Camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 dir = hit.point - transform.position;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
                else
                {
                    Vector3 CenterPos = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f));
                    transform.LookAt(CenterPos);
                }
            }
            else
            {
                UI_Crosshair.enabled = false;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
        else
        {
            if (LockOnSystem._SelectLockOnTarget != null)
            {
                transform.LookAt(LockOnSystem._SelectLockOnTarget.transform.position);
            }
        }
    }
}
