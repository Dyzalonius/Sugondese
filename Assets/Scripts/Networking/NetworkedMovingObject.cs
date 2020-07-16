using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkedMovingObject : NetworkedObject, IPunObservable
{
    public override int ViewId => throw new System.NotImplementedException();

    public override bool IsMine => throw new System.NotImplementedException();

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}