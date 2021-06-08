using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    public float Time;
    private void OnEnable() {
        Invoke("AutoDisable",Time);
    }

    void AutoDisable(){
        this.gameObject.SetActive(false);
    }
}
