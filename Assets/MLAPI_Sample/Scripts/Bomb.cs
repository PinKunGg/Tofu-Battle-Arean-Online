using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Bomb : NetworkBehaviour
{
    private Renderer bombRenderer;
    private NetworkVariableColor bombColor = new NetworkVariableColor();

    void Awake()
    {
        bombRenderer = GetComponent<Renderer>();
    }

    public override void NetworkStart()
    {
        if (!IsServer) { return; }
        bombColor.Value = Random.ColorHSV();
        //if (IsOwnedByServer)
        //{
        //    bombColor.Value = Color.yellow;
        //}
        //else
        //{
        //    bombColor.Value = Color.green;
        //}
    }

    public void SetBombColor(Color newColor)
    {
        bombColor.Value = newColor;
    }

    public void OnEnable()
    {
        bombColor.OnValueChanged += OnBombColorChanged;
    }

    public void OnDisable()
    {
        bombColor.OnValueChanged -= OnBombColorChanged;
    }

    void OnBombColorChanged(Color color, Color newColor)
    {
        if (!IsClient) { return; }
        bombRenderer.material.SetColor("_Color", newColor);
    }

    [ServerRpc]
    private void DestroyBombServerRpc()
    {
        Destroy(gameObject);
    }

    public void DestroyBomb()
    {
        DestroyBombServerRpc();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) { return; }
        if (collision.gameObject.tag == "Player")
        {
            DestroyBomb();
        }
    }
}
