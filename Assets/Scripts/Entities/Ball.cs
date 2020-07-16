using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 30f; // in km/h

    private Vector3 direction;
    private float speed;

    public PlayerController Thrower { get; private set; }

    public bool CanBePickedUp { get; private set; }

    private void Awake()
    {
        CanBePickedUp = true;
    }

    private void Update()
    {
        // Check if ball bounces of a wall
    }

    private void FixedUpdate()
    {
        // Calculate speed in meters per fixed delta time
        speed = movementSpeed / 3.6f * Time.fixedDeltaTime;
        transform.position += direction * speed;
    }

    public void Pickup()
    {
        Destroy(gameObject);
    }

    public void Throw(Vector3 direction, PlayerController thrower)
    {
        this.direction = direction;
        Thrower = thrower;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

                if (Thrower == playerController) { return; }

                if (CanBePickedUp)
                {
                    playerController.PickupBall(this);
                    Pickup();
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
