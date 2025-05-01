using UnityEngine;
using Photon.Pun;

public class SitOnSofa : MonoBehaviourPun
{
    public Transform player;
    public Transform cameraTransform;
    public string sofaKeyword = "Sofa";
    public string chairKeyword = "Chair";
    public float sitHeightOffset = 0.3f;

    public LayerMask interactableLayers;

    private CharacterController characterController;
    private bool isSitting = false;
    private Transform currentSofa = null;
    public CharacterMovement characterMovementScript;

    private PlayerData playerData;

    public float maxDistance;
    

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
        maxDistance = playerData.playerRayLength;
        

        characterController = player.GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController not found on the player!");
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;

        RaycastHit hitInfo;
        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, interactableLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }

        // Sit or stand
        // Press Y on the keyboard or X on the controller to sit or stand up
        if (ButtonMapping.Instance.GetActionDown("Y") && hitInfo.collider != null)
        {
            Transform hitTransform = hitInfo.transform;

            if (!isSitting && (hitTransform.name.Contains(sofaKeyword) || hitTransform.name.Contains(chairKeyword)))
            {
                TrySit(hitTransform, hitInfo.point);
            }
            else if (isSitting)
            {
                StandUp();
            }
        }
    }

    void TrySit(Transform hitTransform, Vector3 hitPoint)
    {
        SofaSeat seat = hitTransform.GetComponent<SofaSeat>();
        if (seat != null && seat.IsOccupied)
        {
            Debug.Log("Sofa already occupied.");
            return;
        }

        if(characterMovementScript != null){
            characterMovementScript.enabled = false; // Disable movement script while sitting
        }

        player.position = hitPoint + Vector3.up * sitHeightOffset;

        isSitting = true;
        currentSofa = hitTransform;

        if (seat != null)
            seat.IsOccupied = true;

        Debug.Log("Sat on: " + hitTransform.name);
    }

    void StandUp()
    {
        isSitting = false;

        if (currentSofa != null)
        {
            SofaSeat seat = currentSofa.GetComponent<SofaSeat>();
            if (seat != null)
                seat.IsOccupied = false;
        }

        if(characterMovementScript != null){
            characterMovementScript.enabled = true; // Enable movement script while sitting
        }

        currentSofa = null;
        Debug.Log("Stood up.");
    }
}
