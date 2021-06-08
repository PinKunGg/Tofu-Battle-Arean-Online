using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Explosion : NetworkBehaviour
{
    public float delay = 3f;
    public float radius = 3;
    public float force = 700;
    public ParticleSystem effect;
    float countdown;
    bool hasExploded = false;
    public AudioClip bombClip;

    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        // show effect
        ParticleSystem particle = Instantiate(effect,transform.position, transform.rotation);
        GetComponent<AudioSource>().PlayOneShot(bombClip);
        GetComponent<Renderer>().enabled = false;

        // get nearby objects (list ของ object ที่ touch)
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.gameObject.tag == "Player")
            {
                Debug.Log("BOOM!");
                //Destroy(nearbyObject.gameObject);
                if (nearbyObject.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    //nearbyObject.gameObject.GetComponent<ScoreScript>().ChangeHealth(-5);
                }
            }
        }

        // remove grenade
        Destroy(gameObject, 1f);
    }
}
