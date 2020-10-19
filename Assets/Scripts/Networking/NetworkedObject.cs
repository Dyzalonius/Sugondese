using Photon.Pun;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedObject : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
    {
        [SerializeField]
        private bool synchronizePosition = true;

        [SerializeField]
        private bool interpolatePosition = false;

        public PhotonView PhotonView { get; private set; }
        public bool IsMine { get { return PhotonView.IsMine; } }
        public int ViewId { get { return PhotonView.ViewID; } }
        public float MovementSpeedInMetersPerSecond { private get; set; }

        public OnInstantiateEvent OnInstantiate;

        private Vector3 networkPosition;
        private Vector3 movement;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            // 1: Lerp
            if (!IsMine && synchronizePosition && interpolatePosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * MovementSpeedInMetersPerSecond);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!synchronizePosition)
            {
                return;
            }

            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
            }
            else
            {
                Vector3 receivedPosition = (Vector3)stream.ReceiveNext();

                if (interpolatePosition)
                {
                    // Update networked position
                    networkPosition = receivedPosition;

                    // Add lag compensation (prediction based on movement)
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    networkPosition += (movement * lag);
                    movement = networkPosition - transform.position;
                }
                else
                {
                    transform.position = receivedPosition;
                }
            }
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] data = info.photonView.InstantiationData;

            // Replace last entry of data (timestamp) with timeDiff
            int timeStamp = (int)data[data.Length - 1];
            int timeDiff = PhotonNetwork.ServerTimestamp - timeStamp;
            data[data.Length - 1] = timeDiff;

            OnInstantiate.Invoke(data);
        }
    }
}