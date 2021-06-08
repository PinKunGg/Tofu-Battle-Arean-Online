using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using UnityEngine.UI;

public class ScoreScript : NetworkBehaviour
{
    Text p1HealthText;
    Text p2HealthText;
    int p1Score = 0;
    int p2Score = 0;

    private void Awake()
    {
        p1HealthText = GameObject.Find("P1Text").GetComponent<Text>();
        p2HealthText = GameObject.Find("P2Text").GetComponent<Text>();
    }

    [SerializeField]
    private NetworkVariableInt healthValue = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 100);

    private void OnEnable()
    {
        healthValue.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        healthValue.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        if (!IsClient) { return; }

        Debug.LogFormat("{0} : Old:{1} : New:{2}", gameObject.name, oldValue, newValue);

        if (newValue >= oldValue) { return; }

        if (IsOwnedByServer)
        {
            p2Score++;
            p2HealthText.text = "P2:" + p2Score;
        }
        else
        {
            p1Score++;
            p1HealthText.text = "P1:" + p1Score;
        }
    }

    [ServerRpc]
    public void ChangeHealthServerRPC(int value)
    {
        healthValue.Value += value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) { return; }

        if (collision.gameObject.tag == "DeathZone")
        {
            ChangeHealthServerRPC(-10);
            gameObject.GetComponent<PlayerSpawner>().Respawn();
        }else if (collision.gameObject.tag == "Bomb")
        {
            ChangeHealthServerRPC(-2);
        }
    }
}
