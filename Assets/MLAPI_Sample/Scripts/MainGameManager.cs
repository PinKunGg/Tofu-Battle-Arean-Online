using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MainGameManager : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButton();
        }
        else
        {
            StatusLables(); SubmitNewPosition();
        }
        GUILayout.EndArea();
    }

    static void StartButton()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLables()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport : " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode : " + mode);
    }

    static void SubmitNewPosition()
    {
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request"))
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(
                NetworkManager.Singleton.LocalClientId,out var networkClient))
            {
                var player = networkClient.PlayerObject.GetComponent<MainPlayer>();
                if (player)
                {
                    player.Move();
                }
            }
        }
    }
}
