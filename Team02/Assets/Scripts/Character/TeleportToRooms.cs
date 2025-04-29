using UnityEngine;
using Photon.Pun;

public class TeleportToRooms : MonoBehaviourPun
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public float teleportOffset = 0.5f;
    public LayerMask hitLayers;

    private CharacterController characterController;

    private PlayerData playerData;

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        characterController = player.GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController not found on the player!");
        }
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        if (!photonView.IsMine)
        {
            return; // Only teleport the local player's instance
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        player.position = targetPosition + Vector3.up * teleportOffset;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Debug.Log("Player teleported to: " + player.position);
    }

    void Update()
    {
        // Only allow teleportation for the local player
        if (!photonView.IsMine)
        {
            return;
        }

        float maxDistance = playerData.playerRayLenght;
        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;
        RaycastHit hitInfo;

        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        // Press X on the controller or Y on the keyboard to teleport
        if ((Input.GetButtonDown("js2") || Input.GetKeyDown(KeyCode.Y)) && hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("DoorToStudy"))
            {
                Vector3 targetPosition = new Vector3(5, 0, 21);
                TeleportPlayer(targetPosition);
            }
            else if (hitInfo.collider.CompareTag("DoorToLiving"))
            {
                Vector3 targetPosition = new Vector3(10, 0, 9);
                TeleportPlayer(targetPosition);
            }
            else if (hitInfo.collider.CompareTag("DoorToLiving2"))
            {
                Vector3 targetPosition = new Vector3(10, 0, -9);
                TeleportPlayer(targetPosition);
            }
            else if (hitInfo.collider.CompareTag("DoorToGame"))
            {
                Vector3 targetPosition = new Vector3(10, 0, -20);
                TeleportPlayer(targetPosition);
            }
        }
    }
}
