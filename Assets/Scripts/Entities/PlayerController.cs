using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 15f; // in km/h

    [SerializeField]
    private GameObject prefabBall = null;

    private Vector3 inputMovement;
    private Vector3 inputAim;
    private float speed;
    private List<Ball> balls = new List<Ball>();

    private void Update()
    {
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

    public void PickupBall(Ball ball)
    {
        ball.transform.parent = transform;
        balls.Add(ball);
    }

    private void ThrowBall()
    {
        if (balls.Count > 0)
        {
            Ball ball = balls[0];
            balls.RemoveAt(0);

            ball = Instantiate(prefabBall, transform.position, Quaternion.identity).GetComponent<Ball>();
            ball.Throw(inputAim, this);
        }
    }
}
