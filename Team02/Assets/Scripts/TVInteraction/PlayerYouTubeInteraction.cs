using UnityEngine;
using Photon.Pun;

public class PlayerYouTubeInteraction : MonoBehaviourPun
{
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";
    private float maxDistance;
    public LayerMask hitLayers;

    private PlayerData playerData;

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        maxDistance = playerData.playerRayLength;

        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) &&
                hitInfo.collider.CompareTag(InteractableTag) &&
                hitInfo.collider.gameObject.name.Contains("TV"))
            {
                YouTubeVideoCanvas videoCanvas = hitInfo.collider.GetComponentInChildren<YouTubeVideoCanvas>();
                if (videoCanvas != null)
                {
                    Debug.Log("Hit TV: " + hitInfo.collider.gameObject.name);
                    videoCanvas.ToggleVideo();
                }
            }
        }
    }
}
