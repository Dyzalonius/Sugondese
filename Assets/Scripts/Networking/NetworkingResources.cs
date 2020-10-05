using Photon.Pun;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Dyzalonius.Sugondese.Networking
{
    /// <summary></summary>
    public class NetworkingResources : MonoBehaviour, IPunPrefabPool
    {
        [SerializeField, Tooltip("Prefabs of objects that need to be shared over network")]
        private GameObject[] prefabsNetworkedObjects = new GameObject[0];

        private Dictionary<string, ConcurrentQueue<GameObject>> networkedObjectsPool = new Dictionary<string, ConcurrentQueue<GameObject>>();

        private void Awake()
        {
            PopulateNetworkedObjectsPool();

            PhotonNetwork.PrefabPool = this;
        }

        /// <summary>Adds entries of prefabsNetworkedObjects to networkedObjectsPool if valid entry</summary>
        private void PopulateNetworkedObjectsPool()
        {
            foreach (GameObject prefab in prefabsNetworkedObjects)
            {
                // Make sure prefab root object is set inactive
                if (prefab.activeSelf)
                {
                    Debug.LogWarning("Prefab with name " + prefab.name + " was active which is not complient with photon instantiation rules");
                    prefab.SetActive(false);
                }

                // Make sure prefab has photon view with takeover ownership option
                PhotonView photonView = prefab.GetComponent<PhotonView>();
                if (photonView == null || photonView.OwnershipTransfer != OwnershipOption.Takeover)
                {
                    Debug.LogError("Prefab with name " + prefab.name + " either has no PhotonView or doesn't have the takeover ownership option enabled");
                    continue;
                }

                // Add prefab to networkedObjectsPool
                networkedObjectsPool.Add(prefab.name, new ConcurrentQueue<GameObject>());
            }
        }

        /// <summary></summary>
        public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
        {
            // Exit if the prefabName does not exist in the networkedObjectsPool
            if (!networkedObjectsPool.ContainsKey(prefabName))
            {
                Debug.LogError("Prefab with name " + prefabName + " cannot be instantiated because it doesn't exist in the networkedObjectsPool.");
                return null;
            }

            GameObject instance;

            // Try dequeuing a pooled networked object, otherwise instantiate a new one
            if (networkedObjectsPool[prefabName].TryDequeue(out instance))
            {
                instance.transform.position = position;
                instance.transform.rotation = rotation;
            }
            else
            {
                // Find prefab and instantiate a new networked object
                foreach (GameObject prefab in prefabsNetworkedObjects)
                {
                    if (prefab.name == prefabName)
                    {
                        instance = Instantiate(prefab, position, rotation);
                        instance.name = prefab.name;
                        break;
                    }
                }
            }

            return instance;
        }

        /// <summary></summary>
        public void Destroy(GameObject gameObject)
        {
            // Exit if the gameObject name does not exist in the networkedObjectsPool
            if (!networkedObjectsPool.ContainsKey(gameObject.name))
            {
                Debug.LogError("Gameobject with name " + gameObject.name + "cannot be destroyed because it doesn't exist in the networkedObjectsPool.");
                return;
            }

            // Destroy gameobject normally and exit if player destroys object when not in a room
            /*if (false) //PhotonNetwork.NetworkClientState != ClientState.Joined)
            {
                GameObject.Destroy(gameObject);
                return;
            }*/

            //if the gameobject to be destroyed is in the resource dictionary it can be reset and enqueued for reusing
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);

            //transfer ownership to scene while this gameobject is inactive
            gameObject.GetComponent<PhotonView>().TransferOwnership(0);
            networkedObjectsPool[gameObject.name].Enqueue(gameObject);
        }
    }
}