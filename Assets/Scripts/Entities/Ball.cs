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

        [SerializeField]
        private float speedTweenTime = 1f; // in seconds

        [SerializeField]
        private float speedFactorOnHit = 0.5f;

        [SerializeField]
        private SpriteRenderer ballRenderer = null;

        private Vector3 direction;
        private float speedPerTick;
        private float movementSpeedCurrent;

        public PlayerController Thrower { get; private set; }
        public bool CanBePickedUp { get; private set; }
        public NetworkedObject NetworkedObject { get; private set; }
        public BallType BallType { get { return ballType; } }
        public Vector3 Direction { get { return direction; } }

        private void Awake()
        {
            CanBePickedUp = false;
            NetworkedObject = GetComponent<NetworkedObject>();
            NetworkedObject.OnInstantiate.AddListener(Throw);
        }

        private void FixedUpdate()
        {
            // Calculate speed in meters per fixed delta time
            speedPerTick = movementSpeedCurrent / 3.6f * Time.fixedDeltaTime;
            transform.position += direction * speedPerTick;
        }

        public void Hit(Vector3 hitPosition, Vector3 directionAfterHit, int timeDiff)
        {
            direction = directionAfterHit;
            movementSpeedCurrent *= speedFactorOnHit;
            CanBePickedUp = true;
            transform.position = hitPosition;

            // Account for timediff between clients
            float tweenTimePast = Mathf.Clamp(timeDiff / 1000, 0f, speedTweenTime);
            float tweenTimeLeft = Mathf.Clamp(speedTweenTime - tweenTimePast, 0f, speedTweenTime);
            float movementSpeedAfterTime = movementSpeedCurrent * tweenTimeLeft / speedTweenTime; //TODO: make sure this is calculated properly!
            float averageSpeedOverTimeDiffInMetersPerSecond = (movementSpeedCurrent + movementSpeedAfterTime) / 2 / 3.6f;
            float distanceTravelledOverTimeDiff = averageSpeedOverTimeDiffInMetersPerSecond * tweenTimePast;

            // !!! Temporarily commented out to test if my method works at all.
            transform.position += direction * distanceTravelledOverTimeDiff;
            LeanTween.value(movementSpeedAfterTime, 0f, tweenTimeLeft).setOnUpdate(val => movementSpeedCurrent = val);
        }

        public void Hide()
        {
            ballRenderer.enabled = false;
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
            movementSpeedCurrent = movementSpeed;
            Thrower = thrower;
            Thrower.ThrowBallLocal(BallType);

            // Account for timediff between clients
            speedPerTick = movementSpeedCurrent / 3.6f;
            transform.position += this.direction * speedPerTick * timeDiff / 1000;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Player":
                    PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

                    if (Thrower == playerController) { return; }
                    if (!playerController.NetworkedObject.IsMine) { return; }

                    if (CanBePickedUp)
                    {
                        playerController.PickUpBall(this);
                    }
                    else
                    {
                        playerController.HitBall(this);
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