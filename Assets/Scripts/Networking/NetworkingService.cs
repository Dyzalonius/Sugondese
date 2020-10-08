using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    public class NetworkingService : MonoBehaviour, IOnEventCallback
    {
        [Header("Settings")]
        [SerializeField, Tooltip("How often to send packets")]
        private int sendRate = 10;

        [SerializeField, Tooltip("How often to receive packets")]
        private int serializationRate = 10;

        [SerializeField]
        private byte maxPlayersInRoom = 10;

        public byte MaxPlayersInRoom { get { return maxPlayersInRoom; } }
        public static NetworkingService Instance { get; private set; }

        private NetworkingResources networkingResources;
        private const byte pickUpBallEventCode = 1;

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
            PhotonNetwork.SendRate = sendRate;
            PhotonNetwork.SerializationRate = serializationRate;
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.ConnectUsingSettings();
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            return PhotonNetwork.Instantiate(prefabName, position, rotation, 0, new object[] { PhotonNetwork.ServerTimestamp });
        }

        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, object[] data)
        {
            // Add timestamp at the end of the data
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = PhotonNetwork.ServerTimestamp;

            return PhotonNetwork.Instantiate(prefabName, position, rotation, 0, data);
        }

        public void Destroy(NetworkedObject obj)
        {
            PhotonNetwork.Destroy(obj.gameObject);
        }

        public GameObject Find(int viewID)
        {
            return PhotonView.Find(viewID).gameObject;
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            object[] data;
            int ballViewID;

            switch (eventCode)
            {
                case pickUpBallEventCode:
                    data = (object[])photonEvent.CustomData;
                    ballViewID = (int)data[0];
                    int pickerViewID = (int)data[1];
                    ReceivePickUpBallEvent(ballViewID, pickerViewID);
                    break;
            }
        }

        public void SendPickUpBallEvent(int ballViewID, int pickerID)
        {
            object[] data = new object[] { ballViewID, pickerID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(pickUpBallEventCode, data, raiseEventOptions, SendOptions.SendReliable);
        }

        private void ReceivePickUpBallEvent(int ballViewID, int pickerID)
        {
            Ball ball = PhotonView.Find(ballViewID)?.GetComponent<Ball>();
            PlayerController picker = PhotonView.Find(pickerID)?.GetComponent<PlayerController>();

            if (ball == null || picker == null)
            {
                Debug.LogWarning("Can't find ball with id " + ballViewID + " or picker with id " + pickerID );
                return;
            }

            picker.PickupBallLocal(ball);
        }

        private void OnValidate()
        {
            if (sendRate < serializationRate)
            {
                sendRate = serializationRate;
            }
        }
    }
}