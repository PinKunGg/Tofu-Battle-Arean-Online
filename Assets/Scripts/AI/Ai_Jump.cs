using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Jump : MonoBehaviour
{
    public Transform JumpChecker;
    Rigidbody rb;
    RaycastHit hit;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if(Physics.SphereCast(JumpChecker.transform.position,1f,transform.forward,out hit,1f)){
            if(hit.collider.CompareTag("JumpStep")){
                JumpPoint JP = hit.collider.GetComponent<JumpPoint>();
                rb.AddForce(new Vector3(rb.velocity.x,JP.JumpForceY,JP.JumpForceZ),ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(JumpChecker.transform.position,1f);
    }
}
