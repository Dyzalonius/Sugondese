using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks
{
    [Header("References")]
    [SerializeField]
    private ClientList clientList;

    [SerializeField]
    private PhotonView photonView;

    [HideInInspector]
    public static LobbyManager Instance { get; private set; } // Static singleton

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

    public void OnFriendListUpdate(List<FriendInfo> friendList) { }

    public void OnCreatedRoom() { }

    public void OnCreateRoomFailed(short returnCode, string message) { }

    public void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Add("isReady", false);
        clientList.UpdateList();
    }

    public void OnJoinRoomFailed(short returnCode, string message) { }

    public void OnJoinRandomFailed(short returnCode, string message) { }

    public void OnLeftRoom() { }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) { }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        clientList.UpdateList();
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) { }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isReady"))
        {
            ClientItem clientItem = clientList.GetClientItem(targetPlayer);
            clientItem.DisplayIsReady((bool)changedProps["isReady"]);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            bool allReady = true;

            foreach (var pair in PhotonNetwork.CurrentRoom.Players)
            {
                if (!(bool)pair.Value.CustomProperties["isReady"]) //TODO: when master client readys up with another client, this gives nullrefexception
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
            {
                PhotonNetwork.LoadLevel("Game"); //TODO: add a scenemanager.onload for every client, so that they can handle what needs to happen when switching
            }
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }

    public void ClickReadyButton(Photon.Realtime.Player player, bool isReady)
    {
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table.Add("isReady", isReady);
        player.SetCustomProperties(table);
    }
}
