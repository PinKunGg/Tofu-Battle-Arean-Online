using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using System.Text;
using MLAPI.Transports.UNET;

public class LoginManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject leaveButton;
    public Text nameInputField;
    public string ipAddress = "127.0.0.1";
    UNetTransport transport;

    public void Client()
    {
        //transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        //transport.ConnectAddress = ipAddress;
        //aa

        if (nameInputField.text == "")
        {
            return;
        }

        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(nameInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void OnIpAddressChanged(string address)
    {
        this.ipAddress = address;
    }

    public void Host()
    {
        if (nameInputField.text == "")
        {
            return;
        }

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        Vector3 spawnPos = GameObject.Find("SpawnPoint").transform.position;
        Quaternion spawnRot = Quaternion.identity;
        NetworkManager.Singleton.StartHost(spawnPos, spawnRot);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId,
        MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        string playerName = Encoding.ASCII.GetString(connectionData);
        bool approve = playerName != nameInputField.text;
        bool createPlayerObject = true;

        Vector3 spawnPos = GameObject.Find("SpawnPoint").transform.position;
        Quaternion spawnRot = Quaternion.identity;

        callback(createPlayerObject, null, approve, spawnPos, spawnRot);
    }

    private void Start()
    {
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
            SetPlayerName(clientId);
        }
    }

    void SetPlayerName(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            return;
        }

        if (!networkClient.PlayerObject.TryGetComponent<SetUpLocalPlayer>(out var mainPlayer))
        {
            return;
        }

        string playerName = nameInputField.text;
        mainPlayer.SetPlayerNameServerRpc(playerName);
    }

    void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(true);
            leaveButton.SetActive(false);
        }
    }

    void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GM.gmInstanse.AllReset();
    }
}
