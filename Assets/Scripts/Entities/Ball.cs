using Dyzalonius.Sugondese.Networking;
using UnityEngine;

namespace Dyzalonius.Sugondese.Entities
{
    public class Ball : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 30f; // in km/h

        [SerializeField]
        private BallType ballType = BallType.Normal;

        public PlayerController Thrower { get; private set; }
        public bool CanBePickedUp { get; private set; }
        public NetworkedObject NetworkedObject { get; private set; }
        public BallType BallType { get { return ballType; } }

        private Vector3 direction;
        private float speed;

        private void Awake()
        {
            CanBePickedUp = true;
            NetworkedObject = GetComponent<NetworkedObject>();
            NetworkedObject.OnInstantiate.AddListener(Throw);
        }

        private void FixedUpdate()
        {
            // Calculate speed in meters per fixed delta time
            speed = movementSpeed / 3.6f * Time.fixedDeltaTime;
            transform.position += direction * speed;
        }

        private void Throw(object[] data)
        {
            int throwerViewID = (int)data[0];
            Vector3 direction = (Vector3)data[1];
            int timeDiff = (int)data[2];
            PlayerController thrower = NetworkingService.Instance.Find(throwerViewID).GetComponent<PlayerController>();

            // Exit if thrower can't be found
            if (thrower == null)
            {
                Debug.Log("Can't find thrower with id " + throwerViewID);
                return;
            }

            this.direction = direction;
            Thrower = thrower;
            Thrower.ThrowBallLocal(BallType);

            // Account for timediff between clients
            speed = movementSpeed / 3.6f;
            transform.position += this.direction * speed * timeDiff / 1000;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Player":
                    PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

                    if (Thrower == playerController) { return; }

                    if (!NetworkedObject.IsMine) { return; }

                    if (CanBePickedUp)
                    {
                        playerController.PickUpBall(this);
                    }
                    else
                    {
                        // hit player
                    }
                    break;

                case "Wall":
                    direction = Vector3.Reflect(direction, other.contacts[0].normal);
                    break;

                default:
                    break;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Player":
                    PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                    if (Thrower == playerController)
                    {
                        Thrower = null;
                    }
                    break;
            }
        }
    }
}