using Dyzalonius.Sugondese.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace Dyzalonius.Sugondese.Entities
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 15f; // in km/h

        private Vector3 inputMovement;
        private Vector3 inputAim;
        private float speed;
        private List<BallType> balls = new List<BallType>();

        public OnBallsChangeEvent OnBallsChange;

        public NetworkedObject NetworkedObject { get; private set; }

        private void Awake()
        {
            NetworkedObject = GetComponent<NetworkedObject>();
            NetworkedObject.MovementSpeedInMetersPerSecond = movementSpeed / 3.6f;
        }

        private void Update()
        {
            // Exit if not local
            if (!NetworkedObject.IsMine)
            {
                return;
            }

            UpdateInputMovement();
            UpdateInputAim();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowBall();
            }
        }

        private void FixedUpdate()
        {
            // Calculate speed in meters per fixed delta time
            speed = movementSpeed / 3.6f * Time.fixedDeltaTime;
            transform.position += inputMovement * speed;
        }

        private void UpdateInputMovement()
        {
            inputMovement = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                inputMovement.y += 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                inputMovement.x -= 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                inputMovement.y -= 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                inputMovement.x += 1;
            }

            inputMovement.Normalize();
        }

        private void UpdateInputAim()
        {
            inputAim = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                inputAim.y += 1;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputAim.x -= 1;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                inputAim.y -= 1;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                inputAim.x += 1;
            }

            inputAim.Normalize();
        }

        private void ThrowBall()
        {
            object[] data = new object[] { NetworkedObject.ViewId, inputAim };
            NetworkingService.Instance.Instantiate("Ball", transform.position, Quaternion.identity, data).GetComponent<Ball>();
        }

        public void ThrowBallLocal(BallType ballType)
        {
            balls.Remove(ballType);
            OnBallsChange.Invoke(balls);
        }

        public void PickUpBall(Ball ball)
        {
            NetworkingService.Instance.SendPickUpBallEvent(ball.BallType, ball.NetworkedObject.ViewId, NetworkedObject.ViewId);
            PickupBallLocal(ball.BallType, ball);
        }

        public void PickupBallLocal(BallType ballType, Ball ball = null)
        {
            // Hide ball model if a ball is passed
            if (ball != null)
            {
                ball.Hide();
            }

            balls.Add(ballType);
            OnBallsChange.Invoke(balls);
        }

        public void HitBall(Ball ball)
        {
            Vector3 newDirection = ball.Direction * -1;
            NetworkingService.Instance.SendHitBallEvent(ball.NetworkedObject.ViewId, NetworkedObject.ViewId, ball.transform.position, newDirection);
            HitBallLocal(ball, ball.transform.position, newDirection, 0);
        }

        public void HitBallLocal(Ball ball, Vector3 ballLocation, Vector3 ballDirection, int timeDiff)
        {
            ball.Hit(ballLocation, ballDirection, timeDiff);
        }
    }
}