using UnityEngine;

public class LightOnOff : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";  // The tag for interactable objects (e.g., TVs, Lamps)
    public float maxDistance;  // Maximum distance for raycast
    public LayerMask hitLayers;  // Layers to interact with

    private PlayerData playerData;  // Reference to PlayerData script

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();  // Get PlayerData component from the player
        maxDistance = playerData.playerRayLenght;  // Set max distance from PlayerData
        // Ensure the LineRenderer is set up
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;

        RaycastHit hitInfo;

        // Perform the raycast
        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;

            // Check for input and correct tag/name
            // Press Y on the keyboard or X on the controller to interact with the lamp
            if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) &&
                hitInfo.collider != null &&
                hitInfo.collider.CompareTag(InteractableTag) &&
                hitInfo.collider.gameObject.name.Contains("Lamp"))
            {
                // Get the Light component from the child
                Light childLight = hitInfo.collider.GetComponentInChildren<Light>();

                if (childLight != null)
                {
                    childLight.enabled = !childLight.enabled;  // Toggle light on/off
                }
            }
        }

        // Show the ray with LineRenderer
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}
