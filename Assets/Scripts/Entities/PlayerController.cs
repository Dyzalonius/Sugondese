﻿using Dyzalonius.Sugondese.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            NetworkingService.Instance.SendPickUpBallEvent(ball.BallType, NetworkedObject.ViewId);
            PickupBallLocal(ball.BallType);

            NetworkingService.Instance.Destroy(ball.NetworkedObject);
        }

        public void PickupBallLocal(BallType ballType)
        {
            balls.Add(ballType);
            OnBallsChange.Invoke(balls);
        }

        public void HitBallLocal(Ball ball)
        {
            // Hit animation
            // Lower health?
        }
    }
}