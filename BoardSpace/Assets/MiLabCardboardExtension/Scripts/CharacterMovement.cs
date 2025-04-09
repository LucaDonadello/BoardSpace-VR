using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviourPun
{
    CharacterController charCntrl;
    [Tooltip("The speed at which the character will move.")]
    public float speed = 5f;
    [Tooltip("The camera representing where the character is looking.")]
    public GameObject cameraObj;
    [Tooltip("Should be checked if using the Bluetooth Controller to move. If using keyboard, leave this unchecked.")]
    public bool joyStickMode;

    // Start is called before the first frame update
    void Start()
    {
        charCntrl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow movement for the local player
        if (!photonView.IsMine)
        {
            return; // If it's not the local player, skip this update
        }

        // Get horizontal and vertical inputs for movement (Keyboard or Joystick)
        float horComp = Input.GetAxis("Horizontal"); // Left/Right (A/D or Left Stick)
        float vertComp = Input.GetAxis("Vertical"); // Forward/Backward (W/S or Up/Down Stick)

        // If using joystick mode, adjust the axes as needed
        if (joyStickMode)
        {
            horComp = Input.GetAxis("Vertical");   // Joystick Forward/Backward
            vertComp = Input.GetAxis("Horizontal");  // Joystick Left/Right (reverse axis for correct direction)
        }

        // Calculate movement direction
        Vector3 moveVect = Vector3.zero;

        // Get look direction based on the camera's forward direction (ignoring Y-axis)
        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f; // Ignore vertical component to keep player movement level
        cameraLook = cameraLook.normalized;

        // Calculate forward and right direction for movement relative to camera
        Vector3 forwardVect = cameraLook;
        Vector3 rightVect = Vector3.Cross(forwardVect, Vector3.up).normalized;

        // Add movement based on user input
        moveVect += rightVect * horComp; // Move sideways (Left/Right)
        moveVect += forwardVect * vertComp; // Move forward/backward

        // Apply speed factor
        moveVect *= speed;

        // Move the character using the CharacterController
        charCntrl.SimpleMove(moveVect); // This applies the movement
    }
}
