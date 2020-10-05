using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ClientList : MonoBehaviour
{
    [SerializeField]
    private ClientItem clientItemPrefab = null;

    private List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>();
    private Dictionary<Photon.Realtime.Player, ClientItem> clientItems = new Dictionary<Photon.Realtime.Player, ClientItem>();

    public void UpdateList()
    {
        // Remove old ones
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (!PhotonNetwork.CurrentRoom.Players.ContainsValue(players[i]))
            {
                RemoveClientItem(players[i]);
            }
        }

        // Add new ones
        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            if (!players.Contains(pair.Value))
            {
                AddClientItem(pair.Value);
            }
        }

        // Update all clientItems
        foreach (var pair in clientItems)
        {
            pair.Value.Setup(pair.Key);
        }
    }

    private void AddClientItem(Photon.Realtime.Player player)
    {
        ClientItem newClientItem = Instantiate(clientItemPrefab, transform).GetComponent<ClientItem>();
        players.Add(player);
        clientItems.Add(player, newClientItem);
    }

    private void RemoveClientItem(Photon.Realtime.Player player)
    {
        ClientItem clientItem = clientItems[player];
        clientItems.Remove(player);
        players.Remove(player);
        Destroy(clientItem.gameObject);
    }
}
