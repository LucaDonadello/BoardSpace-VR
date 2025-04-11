using UnityEngine;
using Photon.Pun;

public class GrabItemVR : MonoBehaviourPun
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public float teleportOffset = 0.5f;
    public LayerMask hitLayers;
    public Vector3 grabLocation = new Vector3(-0.5f, -1f, 1f);
    public string grabbableTag = "Grabbable";
    private GameObject currentObject;
    private Rigidbody currentRb;

    private bool isHolding = false;

    void Update()
    {
        if (!photonView.IsMine) return;

        // Handle release first
        if (isHolding && (Input.GetButtonDown("js0") || Input.GetKeyDown(KeyCode.G)))
        {
            DropObject();
            return; // Early return to prevent grabbing in same frame
        }

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

        if (!isHolding && (Input.GetButtonDown("js0") || Input.GetKeyDown(KeyCode.G)))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Outline outline = hitObject.GetComponent<Outline>();

            if (outline != null && outline.enabled && hitObject.CompareTag(grabbableTag))
            {
                currentObject = hitObject;
                currentRb = currentObject.GetComponent<Rigidbody>();
                isHolding = true;

                if (currentRb != null)
                {
                    currentRb.linearVelocity = Vector3.zero;
                    currentRb.angularVelocity = Vector3.zero;
                    currentRb.useGravity = false;
                    currentRb.isKinematic = true;
                }

                Debug.Log("Grabbed: " + currentObject.name);
            }
        }
    }

    void LateUpdate()
    {
        if (isHolding && currentObject != null)
        {
            // Position the object in front of the camera with the specified offset
            Vector3 targetPos = cameraTransform.position + cameraTransform.TransformDirection(grabLocation);
            Quaternion targetRot = cameraTransform.rotation;

            if (currentRb != null && currentRb.isKinematic)
            {
                currentRb.MovePosition(targetPos);
                currentRb.MoveRotation(targetRot);
            }
            else
            {
                currentObject.transform.position = targetPos;
                currentObject.transform.rotation = targetRot;
            }
        }
    }

    private void DropObject()
    {
        if (currentRb != null)
        {
            currentRb.useGravity = true;
            currentRb.isKinematic = false;
            currentRb.linearVelocity = Vector3.zero;
            currentRb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Dropped: " + currentObject?.name);

        currentObject = null;
        currentRb = null;
        isHolding = false;
    }
}
