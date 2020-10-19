using Dyzalonius.Sugondese.Entities;
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
        private const byte hitBallEventCode = 2;

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

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
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

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            object[] data;
            int ballViewID;

            switch (eventCode)
            {
                case pickUpBallEventCode:
                    data = (object[])photonEvent.CustomData;
                    BallType ballType = (BallType)data[0];
                    ballViewID = (int)data[1];
                    int pickerViewID = (int)data[2];
                    ReceivePickUpBallEvent(ballType, ballViewID, pickerViewID);
                    break;

                case hitBallEventCode:
                    data = (object[])photonEvent.CustomData;
                    ballViewID = (int)data[0];
                    int hitPlayerViewID = (int)data[1];
                    Vector3 ballHitPosition = (Vector3)data[2];
                    Vector3 ballDirectionPostHit = (Vector3)data[3];
                    int hitTimeStamp = (int)data[4];
                    ReceiveHitBallEvent(ballViewID, hitPlayerViewID, ballHitPosition, ballDirectionPostHit, hitTimeStamp);
                    break;
            }
        }

        public void SendPickUpBallEvent(BallType ballType, int ballViewID, int pickerViewID)
        {
            object[] data = new object[] { ballType, ballViewID, pickerViewID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(pickUpBallEventCode, data, raiseEventOptions, SendOptions.SendReliable);
        }

        private void ReceivePickUpBallEvent(BallType ballType, int ballViewID, int pickerViewID)
        {
            PlayerController picker = PhotonView.Find(pickerViewID)?.GetComponent<PlayerController>();
            Ball ball = PhotonView.Find(ballViewID)?.GetComponent<Ball>();

            if (picker == null)
            {
                Debug.LogWarning("Can't find picker with id " + pickerViewID );
                return;
            }

            // Delete ball if you are the owner
            if (ball.NetworkedObject.IsMine)
            {
                Destroy(ball.NetworkedObject);
            }

            // Call local pickup if you are not the picker
            if (!picker.NetworkedObject.IsMine)
            {
                picker.PickupBallLocal(ballType);
            }
        }

        public void SendHitBallEvent(int ballViewID, int hitPlayerViewID, Vector3 ballHitPosition, Vector3 ballDirectionPostHit)
        {
            object[] data = new object[] { ballViewID, hitPlayerViewID, ballHitPosition, ballDirectionPostHit, PhotonNetwork.ServerTimestamp };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(hitBallEventCode, data, raiseEventOptions, SendOptions.SendReliable);
        }

        private void ReceiveHitBallEvent(int ballViewID, int hitPlayerViewID, Vector3 ballHitPosition, Vector3 ballDirectionPostHit, int hitTimeStamp)
        {
            Ball ball = PhotonView.Find(ballViewID)?.GetComponent<Ball>();
            PlayerController hitPlayer = PhotonView.Find(hitPlayerViewID)?.GetComponent<PlayerController>();

            if (ball == null || hitPlayer == null)
            {
                Debug.LogWarning("Can't find ball with id " + ballViewID + " or hit player with id " + hitPlayerViewID);
                return;
            }

            hitPlayer.HitBallLocal(ball, ballHitPosition, ballDirectionPostHit, PhotonNetwork.ServerTimestamp - hitTimeStamp);
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