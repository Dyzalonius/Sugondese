using UnityEngine;

[DisallowMultipleComponent]
public abstract class NetworkedObject : MonoBehaviour
{
    public abstract int ViewId { get; }

    public abstract bool IsMine { get; }
}
