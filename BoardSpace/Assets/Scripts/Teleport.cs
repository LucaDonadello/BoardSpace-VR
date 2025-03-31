using UnityEngine;

public class Teleport : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public string teleportTag = "Teleportable";
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

            if (Input.GetButtonDown("js2") && hitInfo.collider != null && hitInfo.collider.CompareTag(teleportTag)) //js0 windows
            {
                TeleportPlayer(hitInfo.point);
            }
    }
}
