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

        private Vector3 direction;
        private float speedPerTick;
        private float movementSpeedCurrent;

        public PlayerController Thrower { get; private set; }
        public bool CanBePickedUp { get; private set; }
        public NetworkedObject NetworkedObject { get; private set; }
        public BallType BallType { get { return ballType; } }

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

        public void Hit(Vector3 hitPosition, int timeDiff)
        {
            direction *= -1;
            movementSpeedCurrent *= speedFactorOnHit;
            CanBePickedUp = true;
            transform.position = hitPosition;

            // Account for timediff between clients
            float tweenTimeLeft = speedTweenTime - (timeDiff / 1000);
            float movementSpeedAfterTime = movementSpeedCurrent * tweenTimeLeft / speedTweenTime;

            // Calculate average speed over timediff
            float averageSpeedOverTimeDiffInMetersPerSecond = (movementSpeedCurrent + movementSpeedAfterTime) / 2 / 3.6f;

            // Calculate travelled distance using speed and time
            float distanceTravelled = averageSpeedOverTimeDiffInMetersPerSecond * timeDiff / 1000;

            // !!! Temporarily commented out to test if my method works at all.
            //transform.position += direction * distanceTravelled;
            //LeanTween.value(movementSpeedAfterTime, 0f, tweenTimeLeft).setOnUpdate(val => movementSpeedCurrent = val);

            // !!! Temporarily method to test if the tween/timediff method works. It's not fully working
            if (timeDiff == 0)
            {
                LeanTween.value(movementSpeedCurrent, 0f, speedTweenTime).setOnUpdate(val => movementSpeedCurrent = val);
            }
            else
            {

                movementSpeedCurrent = 0f;
                transform.position += direction * averageSpeedOverTimeDiffInMetersPerSecond * speedTweenTime;
            }
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

                    if (!NetworkedObject.IsMine) { return; }

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