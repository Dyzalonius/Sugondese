using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ClientItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TextMeshProUGUI clientNameText = null;

    [SerializeField]
    private TextMeshProUGUI readyText = null;

    public void Setup(Player player)
    {
        clientNameText.text = player.NickName;
        clientNameText.fontStyle = player.IsLocal ? FontStyles.Bold : FontStyles.Normal;

        bool isMasterClient = player.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId; // Alternative to player.isMasterClient, because that variable isn't always correct
        bool isReady = player.CustomProperties.ContainsKey("isReady") ? (bool)player.CustomProperties["isReady"] : false;
        readyText.text = isMasterClient ? "Host" : isReady ? "Ready" : "Not Ready";
        readyText.fontStyle = player.IsLocal ? FontStyles.Bold : FontStyles.Normal;
    }
}
