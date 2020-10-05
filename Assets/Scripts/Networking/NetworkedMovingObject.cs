using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkedMovingObject : NetworkedObject, IPunObservable
{
    [Header("Settings")]
    [SerializeField]
    private bool synchronizePosition = false;

    [SerializeField]
    private bool synchronizeRotation = false;

    [SerializeField]
    private bool synchronizeScale = false;

    public override bool IsMine { get { return PhotonView.IsMine; } }
    public override int ViewId { get { return PhotonView.ViewID; } }
    public PhotonView PhotonView { get; private set; }

    private float distance;
    private float angle;
    private Vector3 direction;
    private Vector3 networkPosition;
    private Vector3 storedPosition;
    private Quaternion networkRotation;
    private bool firstTake;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    public void Update()
    {
        //only synchronize transform if options are set and this isn't our object
        bool canSynchronize = synchronizePosition || synchronizeRotation || synchronizeScale;
        if (canSynchronize && !PhotonView.IsMine)
        {
            float tickTime = 1.0f / PhotonNetwork.SerializationRate;
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, distance * tickTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, angle * tickTime);
        }
    }

    /// <summary>Used for storing the synchronized transform values gained from the server so we can linearly interpolate towards these during game frames</summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (synchronizePosition)
            {
                direction = transform.position - storedPosition;
                storedPosition = transform.position;

                stream.SendNext(transform.position);
                stream.SendNext(direction);
            }

            if (synchronizeRotation)
            {
                stream.SendNext(transform.rotation);
            }

            if (synchronizeScale)
            {
                stream.SendNext(transform.localScale);
            }
        }
        else
        {
            if (synchronizePosition)
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                direction = (Vector3)stream.ReceiveNext();

                if (firstTake)
                {
                    transform.position = networkPosition;
                    distance = 0f;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    networkPosition += direction * lag;
                    distance = Vector3.Distance(transform.position, networkPosition);
                }
            }

            if (synchronizeRotation)
            {
                networkRotation = (Quaternion)stream.ReceiveNext();

                if (firstTake)
                {
                    angle = 0f;
                    transform.rotation = networkRotation;
                }
                else
                {
                    angle = Quaternion.Angle(transform.rotation, networkRotation);
                }
            }

            if (synchronizeScale)
            {
                transform.localScale = (Vector3)stream.ReceiveNext();
            }

            if (firstTake)
            {
                firstTake = false;
            }
        }
    }
}