using Photon.Pun;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedObject : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
    {
        [SerializeField]
        private bool synchronizePosition = true;

        public PhotonView PhotonView { get; private set; }
        public bool IsMine { get { return PhotonView.IsMine; } }
        public int ViewId { get { return PhotonView.ViewID; } }

        public OnInstantiateEvent OnInstantiate;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
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
                transform.position = (Vector3)stream.ReceiveNext();
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