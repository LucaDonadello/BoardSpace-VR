using UnityEngine;
using Photon.Pun;

public class PlayerMusicInteraction : MonoBehaviourPun
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";  // The tag for the interactable objects (TVs)
    public float maxDistance = 10f;  // Maximum distance for raycast
    public LayerMask hitLayers;  // Layers to interact with (TVs or other interactables)

    private bool isPlaying = false;

    void Start()
    {
        // Ensure the LineRenderer is set up
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    void Update()
    {
        // Only process interaction for the local player
        if (!photonView.IsMine)
        {
            return;
        }

        Vector3 startPosition = player.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;

        RaycastHit hitInfo;

        // Perform the raycast
        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }
        // Show the ray with LineRenderer
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        // Check if the hit object has the tag "TV"
        if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) && hitInfo.collider != null && hitInfo.collider.CompareTag(InteractableTag) && hitInfo.collider.gameObject.name.Contains("TV"))
        {
            TVMusicControl tvMusic = hitInfo.transform.GetComponent<TVMusicControl>();

            if (tvMusic != null)
            {
                if (!isPlaying)
                {
                    tvMusic.PlayNextTrack();  // Start playing the music on the TV
                    isPlaying = true;
                    Debug.Log("Playing music on TV.");
                }
                else
                {
                    tvMusic.StopMusic();  // Stop music on the TV
                    isPlaying = false;
                    Debug.Log("Stopped music on TV.");
                }
            }
        }
    }
}