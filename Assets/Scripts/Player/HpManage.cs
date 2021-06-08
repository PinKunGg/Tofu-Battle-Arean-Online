using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class HpManage : NetworkBehaviour
{
    public Slider HpBar;
    public Image FillArea;
    float MaxHp = 100f;

    [SerializeField]
    private NetworkVariableFloat Hp = new NetworkVariableFloat(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 0);

    private void OnEnable()
    {
        HpBar.maxValue = MaxHp;
        SetHealServerRpc(MaxHp);
        Hp.OnValueChanged += OnHpChange;
    }

    private void OnDisable()
    {
        Hp.OnValueChanged -= OnHpChange;
    }

    private void OnHpChange(float oldValue, float newValue)
    {
        if (!IsClient) { return; }
        HpBar.value = Hp.Value;
    }

    bool islogin;
    public override void NetworkStart()
    {
        HpBar.maxValue = MaxHp;
        SetHealServerRpc(MaxHp);
        islogin = true;

        if (this.gameObject.tag == "PlayerA")
        {
            FillArea.color = Color.green;
        }
        else
        {
            FillArea.color = Color.red;
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(float Damage)
    {
        Hp.Value -= Damage;

        if (HpBar.value <= 0)
        {
            Debug.Log("Player Death");
            PlayerDeathClientRpc();
        }
    }

    [ClientRpc]
    public void PlayerDeathClientRpc()
    {
        GM.gmInstanse.Player = this.gameObject;
        GM.gmInstanse.RespawnServerRpc();
        this.gameObject.SetActive(false);
    }

    [ServerRpc]
    public void HealServerRpc(float HealAmout)
    {
        if (Hp.Value + HealAmout <= MaxHp)
        {
            Hp.Value += HealAmout;
        }
        else
        {
            Hp.Value = MaxHp;
        }
    }

    [ServerRpc]
    public void SetHealServerRpc(float HealAmout)
    {
        if (HealAmout <= MaxHp)
        {
            Hp.Value = HealAmout;
            HpBar.value = HealAmout;
        }
        else
        {
            Hp.Value = MaxHp;
            HpBar.value = MaxHp;
        }
    }
}
