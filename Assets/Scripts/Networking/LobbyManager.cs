using Dyzalonius.Sugondese.Networking;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("References")]
    [SerializeField]
    private ClientList clientList = null;

    [SerializeField]
    private TMP_InputField playerNameInput = null;

    [SerializeField]
    private TMP_InputField roomNameInput = null;

    [SerializeField]
    private Button joinRoomButton = null;

    [SerializeField]
    private Button startGameButton = null;

    [SerializeField]
    private Button readyButton = null;

    [SerializeField]
    private TMP_Text readyButtonText = null;

    [SerializeField]
    private TMP_Text lobbyNameText = null;

    private void Start()
    {
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        joinRoomButton.interactable = true;
        PhotonNetwork.AutomaticallySyncScene = true; //TODO: Should probably be in a different class
    }

    public override void OnJoinedRoom()
    {
        lobbyNameText.text = PhotonNetwork.CurrentRoom.Name;

        SetReady(false);

        UpdateLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateLobby();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobby();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateLobby();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateLobby();
    }

    private void UpdateLobby()
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        clientList.UpdateList();
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = CheckIfAllReady();
        }
    }

    private bool CheckIfAllReady()
    {
        bool allReady = true;

        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            if (!pair.Value.IsMasterClient && (!pair.Value.CustomProperties.ContainsKey("isReady") || !(bool)pair.Value.CustomProperties["isReady"]))
            {
                allReady = false;
                break;
            }
        }

        return allReady;
    }

    public void JoinRoom()
    {
        PhotonNetwork.NickName = playerNameInput.text;

        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, new RoomOptions() { MaxPlayers = NetworkingService.Instance.MaxPlayersInRoom }, TypedLobby.Default);
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ToggleReady()
    {
        bool isReady = !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"];
        SetReady(isReady);
    }

    public void SetReady(bool isReady)
    {
        readyButtonText.text = isReady ? "Unready" : "Ready";

        Hashtable table = new Hashtable();
        table.Add("isReady", isReady);
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
