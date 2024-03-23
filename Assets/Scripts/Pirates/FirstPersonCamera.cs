using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{

    // Variables
    public Transform player;
    public float mouseSensitivity;
    float cameraVerticalRotation = 0f;
    public bool lockHorizontalRotation = false;
    public bool lockVerticalRotation = false;

    bool lockedCursor = true;


    void Start()
    {
        // Lock and Hide the Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        // Collect Mouse Input

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the Camera around its local X axis

        if (!lockVerticalRotation)
        {
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        }

        // Rotate the Player Object and the Camera around its Y axis
        if (!lockHorizontalRotation)
        {
            player.Rotate(Vector3.up * inputX);
        }

        

    }
}