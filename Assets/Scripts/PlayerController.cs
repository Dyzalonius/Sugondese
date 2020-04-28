using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    private Vector3 inputMovement;

    private void Update()
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
    }

    private void FixedUpdate()
    {
        transform.position += inputMovement * movementSpeed;
    }
}
