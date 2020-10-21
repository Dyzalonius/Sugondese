using Photon.Pun;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedObject : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
    {
        [SerializeField]
        private bool syncPosition = true;

        [SerializeField]
        private bool syncPositionInterpolation = false;

        [SerializeField]
        private bool syncDirectionWhenDesynced = false;

        [SerializeField]
        private float maxPositionDesync = 1f;

        [SerializeField]
        private float maxLagForPositionInterpolationLagCompensation = 0.2f;

        public PhotonView PhotonView { get; private set; }
        public bool IsMine { get { return PhotonView.IsMine; } }
        public int ViewId { get { return PhotonView.ViewID; } }
        public float MovementSpeedInMetersPerSecond { private get; set; }
        public Vector3 Direction { private get; set; }

        public OnInstantiateEvent OnInstantiate;

        private Vector3 networkPosition;
        private Vector3 movement;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if (!IsMine && syncPosition && syncPositionInterpolation)
            {
                transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * MovementSpeedInMetersPerSecond);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!syncPosition)
            {
                return;
            }

            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                if (syncDirectionWhenDesynced)
                {
                    stream.SendNext(Direction);
                }
            }
            else
            {
                Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
                Vector3 receivedDirection = Vector3.zero;

                if (syncDirectionWhenDesynced)
                {
                    receivedDirection = (Vector3)stream.ReceiveNext();
                }

                if (syncPositionInterpolation)
                {
                    // Update networked position
                    networkPosition = receivedPosition;

                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                    // Synchronize position if too desynced
                    float distance = (transform.position - networkPosition).magnitude;
                    if (distance > maxPositionDesync)
                    {
                        transform.position = networkPosition;
                    }
                    // Add lag compensation (prediction based on movement) if lag isn't insane
                    else if (lag <= maxLagForPositionInterpolationLagCompensation)
                    {
                        networkPosition += (movement * lag);
                    }
                    movement = networkPosition - transform.position;
                }
                else
                {
                    if (syncDirectionWhenDesynced)
                    {
                        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

                        // Synchronize position if too desynced
                        float distance = (transform.position - receivedPosition).magnitude;
                        if (distance > maxPositionDesync && Direction == receivedDirection)
                        {
                            transform.position = receivedPosition + receivedDirection * MovementSpeedInMetersPerSecond * lag;
                        }
                    }
                    else
                    {
                        transform.position = receivedPosition;
                    }
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

        private void OnValidate()
        {
            if (!syncPosition)
            {
                syncPositionInterpolation = false;
                syncDirectionWhenDesynced = false;
            }
        }
    }
}