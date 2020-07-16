using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField]
    private bool autoConnect = true;

    [HideInInspector]
    public bool isReady = false;

    [HideInInspector]
    public string ClientName;

    [HideInInspector]
    public static Client Instance { get; private set; } // Static singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (autoConnect)
            Connect();
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinOrCreateRoom(string roomName, byte playerCount)
    {
        RoomOptions options = new RoomOptions() { MaxPlayers = playerCount };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public void ToggleReady()
    {
        isReady = !isReady;
    }
}
