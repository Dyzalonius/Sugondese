using Dyzalonius.Sugondese.Networking;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Spawn player");
        PlayerController playerController = NetworkingService.Instance.Instantiate("Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
    }
}
