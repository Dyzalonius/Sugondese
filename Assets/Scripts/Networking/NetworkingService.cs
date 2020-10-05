using Photon.Pun;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    public class NetworkingService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int sendRate = 40;

        [SerializeField]
        private int serializationRate = 20;

        [SerializeField]
        private byte maxPlayersInRoom = 10;

        public byte MaxPlayersInRoom { get { return maxPlayersInRoom; } }

        public static NetworkingService Instance { get; private set; }

        private NetworkingResources networkingResources;

        private void Awake()
        {
            // Create Instance
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                GameObject.Destroy(gameObject);
                return;
            }

            networkingResources = GetComponent<NetworkingResources>();
            //eventHandler = new NetworkingEventHandler(HandleRequest);
            //propertyHandler = new PlayerPropertyHandler();

            PhotonNetwork.SendRate = this.sendRate;
            PhotonNetwork.SerializationRate = this.serializationRate;
            //PhotonNetwork.AddCallbackTarget(this);

            PhotonNetwork.ConnectUsingSettings();

            DontDestroyOnLoad(gameObject);
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            return PhotonNetwork.Instantiate(prefabName, position, rotation);
        }

        public void Destroy(NetworkedObject obj)
        {
        }
    }
}