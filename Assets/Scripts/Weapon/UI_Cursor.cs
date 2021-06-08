using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Cursor : MonoBehaviour
{
    private void Update() {
        this.transform.position = Input.mousePosition;
    }
}
