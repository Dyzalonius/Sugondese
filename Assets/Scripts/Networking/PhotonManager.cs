using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks
{
    [SerializeField]
    private JoinRoomButton joinRoomButton;

    public Client Client;

    [HideInInspector]
    public static PhotonManager Instance { get; private set; } // Static singleton

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

    public virtual void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public virtual void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) { }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) { }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) { }

    public void OnFriendListUpdate(List<FriendInfo> friendList) { }

    public void OnCreatedRoom() { }

    public void OnCreateRoomFailed(short returnCode, string message) { }

    public void OnJoinedRoom() { }

    public void OnJoinRoomFailed(short returnCode, string message) { }

    public void OnJoinRandomFailed(short returnCode, string message) { }

    public void OnLeftRoom() { Debug.LogError("OnLeftRoom"); }
}
