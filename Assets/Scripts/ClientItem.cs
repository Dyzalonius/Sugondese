using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI clientNameText;

    [SerializeField]
    private TextMeshProUGUI readyText;

    [SerializeField]
    private TextMeshProUGUI readyButtonText;

    [SerializeField]
    private Button readyButton;

    private bool isReady;

    public void Setup(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.ContainsKey("isReady"))
            isReady = (bool)player.CustomProperties["isReady"];
        else
            isReady = false; //TODO: Check if this is necessary
        clientNameText.text = "player#" + player.ActorNumber;
        readyButtonText.text = isReady ? "Unready" : "Ready";
        readyText.text = isReady ? "Ready" : "Waiting";


        if (player != PhotonNetwork.LocalPlayer)
            readyButton.gameObject.SetActive(false);
        else
            readyButton.onClick.AddListener(ToggleReady);
    }

    public void ToggleReady()
    {
        isReady = !isReady;
        LobbyManager.Instance.ClickReadyButton(PhotonNetwork.LocalPlayer, isReady);
    }

    // Only called by RPC from LobbyManager
    public void DisplayIsReady(bool isReady)
    {
        readyButtonText.text = isReady ? "Unready" : "Ready";
        readyText.text = isReady ? "Ready" : "Waiting";
    }
}
