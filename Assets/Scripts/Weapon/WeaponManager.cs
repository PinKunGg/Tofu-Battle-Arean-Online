using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class WeaponManager : NetworkBehaviour
{
    public List<GameObject> WeaponList;
    public Queue<GameObject> WeaponQueueList;

    [SerializeField]
    private NetworkVariableInt WeaponIndex = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 0);

    private void OnEnable()
    {
        WeaponIndex.OnValueChanged += OnWeaponIndexChange;
    }

    private void OnDisable()
    {
        WeaponIndex.OnValueChanged -= OnWeaponIndexChange;
    }

    private void OnWeaponIndexChange(int oldValue, int newValue)
    {
        if (!IsClient) { return; }
        WeaponList[oldValue].SetActive(false);
        WeaponList[newValue].SetActive(true);
    }
    private void Awake()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        WeaponQueueList = new Queue<GameObject>();

        foreach (var item in WeaponList)
        {

            WeaponQueueList.Enqueue(item);
        }
    }
    private void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (!Input.GetKey(KeyCode.LeftControl))
        {
            if ((Input.GetAxis("Mouse ScrollWheel") < 0f))
            {
                ChangeWeaponUpServerRpc();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                ChangeWeaponDownServerRpc();
            }
        }
    }
    [ServerRpc]
    void ChangeWeaponUpServerRpc()
    {
        if (WeaponIndex.Value + 1 < WeaponList.Count)
        {
            WeaponIndex.Value++;
        }
    }

    [ServerRpc]
    void ChangeWeaponDownServerRpc()
    {
        if (WeaponIndex.Value - 1 >= 0)
        {
            WeaponIndex.Value--;
        }
    }
}
