using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinRoomButton : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField]
    private TMP_InputField PlayerNameInput;

    [SerializeField]
    private TMP_InputField RoomNameInput;

    [SerializeField]
    private Button button;

    [Header("Settings")]
    [SerializeField]
    private byte RoomPlayerLimit = 4;

    private void Start()
    {
        button.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        button.interactable = true;
        PhotonNetwork.AutomaticallySyncScene = true; //TODO: Should probably be in a different class
    }

    public void OnClick()
    {
        Client.Instance.ClientName = PlayerNameInput.text;
        SceneManager.LoadScene("Lobby");
        Client.Instance.JoinOrCreateRoom(RoomNameInput.text, RoomPlayerLimit);
    }
}
