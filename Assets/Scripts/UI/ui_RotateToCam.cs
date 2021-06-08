using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_RotateToCam : MonoBehaviour
{
    private void Update() {
        if(Camera.main != null){
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
