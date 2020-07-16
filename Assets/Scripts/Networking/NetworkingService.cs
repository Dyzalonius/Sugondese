using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingService : MonoBehaviour
{
    public static NetworkingService Instance { get; private set; }

    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, bool isSceneObject = false)
    {
        return null;
    }

    public void Destroy(NetworkedObject obj)
    {
    }
}
