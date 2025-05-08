using UnityEngine;
using Photon.Pun;

public class GrabItemVR : MonoBehaviourPun
{
    public Transform player;
    public Transform cameraTransform;
    public LayerMask hitLayers;
    public Vector3 grabLocation = new Vector3(-0.5f, -1f, 1f);
    public string grabbableTag = "Grabbable";
    private GameObject currentObject;
    private Rigidbody currentRb;
    public float holdDistance = 3.0f;
    private float originalHoldDistance;
    private bool isHolding = false;
    private PlayerData playerData;
    private SitOnSofa sitOnSofa;
    public AudioClip grabSound;
    private AudioSource audioSource;

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
        sitOnSofa = player.GetComponent<SitOnSofa>();
        originalHoldDistance = holdDistance;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Handle release first
        // Press Y on the controller or G on the keyboard to drop the object
        if (isHolding && ButtonMapping.Instance.GetActionDown("Y"))
        {
            DropObject();
            return;
        }
        else if(isHolding && ButtonMapping.Instance.GetActionDown("A"))
        {
            ThrowObject();
            return;
        }

        //adjust hold distance only when sitting
        if (isHolding && sitOnSofa != null && sitOnSofa.IsSitting)
        {
            float scrollInput = Input.GetAxis("Vertical");
            holdDistance += scrollInput * Time.deltaTime * 1.5f;
            holdDistance = Mathf.Clamp(holdDistance, 0.2f, 3.5f);
        }

        float maxDistance = playerData.playerRayLength;
        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;
        RaycastHit hitInfo;

        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }

        // Press Y on the controller or G on the keyboard to grab the object
        if (!isHolding && ButtonMapping.Instance.GetActionDown("Y"))
        {
            GameObject hitObject = hitInfo.collider.gameObject;

            if (hitObject.CompareTag(grabbableTag))
            {
                currentObject = hitObject;
                currentRb = currentObject.GetComponent<Rigidbody>();
                isHolding = true;
                if (grabSound != null && audioSource != null)
                    audioSource.PlayOneShot(grabSound);

                if (currentRb != null)
                {
                    Vector3 safeGrabPosition = cameraTransform.position + cameraTransform.forward * holdDistance;
                    Quaternion uprightRot = Quaternion.Euler(0f, cameraTransform.eulerAngles.y + 180, 0f);

                    //Move it to safe position BEFORE making it kinematic to avoid game pieces flying everywhere
                    currentRb.position = safeGrabPosition;
                    currentRb.rotation = uprightRot;

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
            //Temporarily disable collider to avoid raycasting object I'm holding
            Collider col = currentObject.GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Vector3 desiredPos = cameraTransform.position + cameraTransform.forward * holdDistance;
            Quaternion uprightRot = Quaternion.Euler(0f, cameraTransform.eulerAngles.y + 180, 0f);

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, playerData.playerRayLength, hitLayers, QueryTriggerInteraction.Ignore))
            {
                float hitDist = Vector3.Distance(cameraTransform.position, hitInfo.point);
                if (hitDist < holdDistance)
                {
                    desiredPos = hitInfo.point - ray.direction * 0.05f; // offset slightly to avoid clipping
                }
            }

            if (currentRb != null && currentRb.isKinematic)
            {
                currentRb.MovePosition(desiredPos);
                currentRb.MoveRotation(uprightRot);
            }
            else
            {
                currentObject.transform.position = desiredPos;
                currentObject.transform.rotation = uprightRot;
            }

            //Reenable collider
            if (col != null)
                col.enabled = true;
        }
    }

    private void DropObject()
    {
        if (currentRb != null)
        {
            currentRb.useGravity = true;
            currentRb.isKinematic = false;
            currentRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            currentRb.interpolation = RigidbodyInterpolation.Interpolate;
            currentRb.linearVelocity = Vector3.zero;
            currentRb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Dropped: " + currentObject?.name);
        SnapToChessSquare snapScript = currentObject.GetComponent<SnapToChessSquare>();
        if (snapScript != null)
            snapScript.SnapToClosest();
            
        currentObject = null;
        currentRb = null;
        isHolding = false;
        holdDistance = originalHoldDistance;
    }

    private void ThrowObject()
    {
        if (currentRb != null)
        {
            currentRb.useGravity = true;
            currentRb.isKinematic = false;
            currentRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            currentRb.interpolation = RigidbodyInterpolation.Interpolate;
            currentRb.linearVelocity = cameraTransform.forward * 3f;
            currentRb.angularVelocity = Random.insideUnitSphere * 2f;
        }

        //tell reset position script only if it's ping pong ball
        if (currentObject != null && currentObject.name.StartsWith("SM_ping_pong_ball"))
        {
            ManualBallReset resetScript = currentObject.GetComponent<ManualBallReset>();
            if (resetScript != null)
                resetScript.RegisterThrow();
        }

        Debug.Log("Threw: " + currentObject?.name);    
        currentObject = null;
        currentRb = null;
        isHolding = false;
        holdDistance = originalHoldDistance;
    }
} 
