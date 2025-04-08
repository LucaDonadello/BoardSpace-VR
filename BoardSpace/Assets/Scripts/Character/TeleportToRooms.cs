using UnityEngine;

public class TeleportToRooms : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public float teleportOffset = 0.5f;
    public LayerMask hitLayers;

    private CharacterController characterController;

    void Start()
    {
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
        float maxDistance = 10;
        Vector3 startPosition = player.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;
        RaycastHit hitInfo;


        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }


        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        if ((Input.GetButtonDown("js2") || Input.GetKeyDown(KeyCode.Y)) && hitInfo.collider != null && hitInfo.collider.CompareTag("DoorToStudy")) //js0 windows
        {
            Vector3 targetPosition = new Vector3(5, 0, 21);
            TeleportPlayer(targetPosition);
        }
        else if ((Input.GetButtonDown("js2") || Input.GetKeyDown(KeyCode.Y)) && hitInfo.collider != null && hitInfo.collider.CompareTag("DoorToLiving")) //js0 windows
        {
            Vector3 targetPosition = new Vector3(10, 0, 9);
            TeleportPlayer(targetPosition);
        }
        else if ((Input.GetButtonDown("js2") || Input.GetKeyDown(KeyCode.Y)) && hitInfo.collider != null && hitInfo.collider.CompareTag("DoorToLiving2")) //js0 windows
        {
            Vector3 targetPosition = new Vector3(10, 0, -9);
            TeleportPlayer(targetPosition);
        }
        else if ((Input.GetButtonDown("js2") || Input.GetKeyDown(KeyCode.Y)) && hitInfo.collider != null && hitInfo.collider.CompareTag("DoorToGame")) //js0 windows
        {
            Vector3 targetPosition = new Vector3(10, 0, -20);
            TeleportPlayer(targetPosition);
        }
    }
}
