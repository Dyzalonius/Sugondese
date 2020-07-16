using Photon.Pun;
using UnityEngine;

public class NetworkingResources : MonoBehaviour, IPunPrefabPool
{
    [SerializeField, Tooltip("Prefabs of objects that need to be shared over network")]
    private GameObject[] networkedObjects = new GameObject[0];

    public new GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }

    public void Destroy(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }
}
