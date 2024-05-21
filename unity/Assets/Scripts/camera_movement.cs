using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float movementSpeed = 10.0f; // Adjust for desired movement speed
    public float rotationSpeed = 100.0f; // Adjust for desired rotation sensitivity

    void Update()
    {
        // WASD movement
        float horizontalMovement = Input.GetAxis("Horizontal") * movementSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * movementSpeed;

        transform.Translate(Vector3.right * horizontalMovement * Time.deltaTime);
        transform.Translate(Vector3.forward * verticalMovement * Time.deltaTime);

        // QE movement on Y axis
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.up * -movementSpeed * Time.deltaTime);
        }

        // Trackpad panning (assuming mouse trackpad)
        if (Input.GetMouseButton(1)) // Check for middle mouse button hold
        {
            float deltaX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float deltaY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, deltaX, Space.World);
            transform.Rotate(Vector3.right, -deltaY, Space.Self); // Rotate around camera's right axis for smoother look
        }
    }
}
