using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_LockOnTarget : MonoBehaviour
{
    public Transform target;
    private void Update() {
        if(target != null){
            transform.position = Camera.main.WorldToScreenPoint(target.position);
        }
        transform.Rotate(0f,0f,100f * Time.deltaTime);
    }
}
