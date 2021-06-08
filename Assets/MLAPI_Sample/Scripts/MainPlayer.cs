using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine.UI;

public class MainPlayer : NetworkBehaviour
{
    public Text namePrefab;
    private Text nameLable;
    string textBoxName = "";

    //private void OnGUI()
    //{
    //    if (IsOwner)
    //    {
    //        textBoxName = GUI.TextField(new Rect(225, 15, 100, 25), textBoxName);
    //        if (GUI.Button(new Rect(330,15,35,35),"Set"))
    //        { playerName.Value = textBoxName; }
    //    }
    //}

    public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariable<string> playerName = new NetworkVariable<string>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly
    }, "Player");

    private void OnEnable()
    {
        playerName.OnValueChanged += OnPlayerNameChanged;
        if (nameLable != null)
        {
            nameLable.enabled = true;
        }
    }

    private void OnDisable()
    {
        playerName.OnValueChanged -= OnPlayerNameChanged;
        if (nameLable != null)
        {
            nameLable.enabled = false;
        }
    }

    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (!IsClient) { return; }

        gameObject.name = newValue;
        Debug.LogFormat("old new : {0} >> new name : {1}", oldValue, newValue);
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;
    }

    public override void NetworkStart()
    {
        //Move();
        //SetPlayerName();
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        nameLable = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as Text;
        nameLable.transform.SetParent(canvas.transform);
    }

    private void OnDestroy()
    {
        if (nameLable != null)
        {
            Destroy(nameLable.gameObject);
        }
    }

    public void SetPlayerName()
    {
        if (!IsOwner) return;
        if (NetworkManager.Singleton.IsServer)
        {
            playerName.Value = "Player1";
        }
        else { playerName.Value = "Player2"; }
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randPos = GetRandPos();
            transform.position = randPos; Position.Value = randPos;
        }else
        {
            ChangePosServerRpc();
        }
    }

    [ServerRpc]
    void ChangePosServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandPos();
    }

    private void Update()
    {
        //transform.position = Position.Value;
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.5f, 0));
        nameLable.text = gameObject.name;
        nameLable.transform.position = nameLabelPos;
    }

    static Vector3 GetRandPos()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3, 3f));
    }

    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;
    Rigidbody rb;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
            return;

        float translation = Input.GetAxis("Vertical");
        if (translation != 0)
        {
            translation *= Time.deltaTime * speed;
            rb.MovePosition(rb.position + this.transform.forward * translation);
        }

        float rotation = Input.GetAxis("Horizontal") ;
        if (rotation != 0)
        {
            rotation *= rotationSpeed;
            Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turn);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}
