using UnityEngine;
using Photon.Pun;

public class XRPhotonAnimatorController : MonoBehaviourPun
{
  private Animator animator;
  private Transform modelTransform;
  private CharacterController characterController;
  private CharacterMovement characterMovement;
  private Transform headTransform;

  private Vector3 previousPosition;
  private float currentSpeed;

  private Transform lastActiveModel;

  public float gravity = -9.81f;
  private Vector3 verticalVelocity;

  void Start()
  {
    if (!photonView.IsMine)
    {
        enabled = false;
        return;
    }

    characterController = GetComponent<CharacterController>();
    headTransform = transform.Find("XRCardboardRig/HeightOffset/Main Camera");
    characterMovement = GetComponent<CharacterMovement>();

    UpdateActiveModel(); // initial grab
    previousPosition = transform.position;
  }

  void Update()
  {
    if (!photonView.IsMine || characterController == null || !characterController.enabled || !characterMovement.enabled) return;

    // 🔁 LIVE skin model detection
    UpdateActiveModel();

    // ✅ Read input
    Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    if (input.sqrMagnitude > 0.01f)
    {
      Vector3 headForward = headTransform.forward;
      headForward.y = 0;
      headForward.Normalize();

      Vector3 headRight = headTransform.right;
      headRight.y = 0;
      headRight.Normalize();

      Vector3 move = headForward * input.y + headRight * input.x;

      // gravity
      if (characterController.isGrounded && verticalVelocity.y < 0)
          verticalVelocity.y = -2f;
      else
          verticalVelocity.y += gravity * Time.deltaTime;

      move += verticalVelocity;

      characterController.Move(move * Time.deltaTime);

      // speed for animation
      float deltaDist = Vector3.Distance(transform.position, previousPosition);
      float deltaSpeed = deltaDist / Time.deltaTime;
      currentSpeed = Mathf.Lerp(currentSpeed, deltaSpeed, Time.deltaTime * 5f);
      previousPosition = transform.position;
    }
    else
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 5f);
    }

    if (animator != null)
    {
      animator.SetFloat("Forward", currentSpeed);
      animator.SetFloat("Turn", 0f);
      animator.SetBool("OnGround", true);
    }
  }

  void LateUpdate()
  {
    if (!photonView.IsMine || modelTransform == null) return;

    // lock model position to prevent float/drift
    modelTransform.localPosition = Vector3.zero;
    modelTransform.localRotation = Quaternion.identity;
  }

  void UpdateActiveModel()
  {
    Transform skinParent = transform.Find("CharacterSkin");
    foreach (Transform child in skinParent)
    {
        if (child.gameObject.activeSelf && child != lastActiveModel)
        {
            modelTransform = child;
            animator = child.GetComponent<Animator>();
            lastActiveModel = child;
            break;
        }
    }
  }

  [PunRPC]
  public void SetParentToCharacterSkin(int skinViewID, int parentViewID)
  {
      PhotonView skinView = PhotonView.Find(skinViewID);
      PhotonView parentView = PhotonView.Find(parentViewID);

      if (skinView != null && parentView != null)
      {
          skinView.transform.SetParent(parentView.transform, true);
          skinView.transform.localPosition = Vector3.zero;
          skinView.transform.localRotation = Quaternion.identity;
      }
  }
}
