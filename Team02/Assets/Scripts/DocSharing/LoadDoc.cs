using UnityEngine;
using System.IO;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class LoadDoc : MonoBehaviourPun
{
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";  // The tag for interactable objects (e.g., TVs, Lamps)
    public LayerMask hitLayers;  // Layers to interact with
    private float maxDistance = 10f;  // Maximum distance for raycast
    private string fileExtension = "image/*";
    private PlayerData playerData;
    [SerializeField] RawImage image;
    //[SerializeField] PDFViewer pdfViewer;
    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
        maxDistance = playerData.playerRayLength;
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        image = FindFirstObjectByType<RawImage>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        maxDistance = playerData.playerRayLength;
        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;

        RaycastHit hitInfo;

        // Perform the raycast
        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
        }
        // Check for input and correct tag/name
        // Press Y on the keyboard or X on the controller to interact with the lamp
        if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) &&
            hitInfo.collider != null &&
            hitInfo.collider.CompareTag(InteractableTag) &&
            hitInfo.collider.gameObject.name.Contains("Load"))
        {
            Debug.Log("LoadDoc: " + hitInfo.collider.gameObject.name);
            // Get the Light component from the child
            if (NativeFilePicker.IsFilePickerBusy())
                return;
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null)
                    Debug.Log("Operation cancelled");
                else
                {
                    byte[] fileData = File.ReadAllBytes(path);
                    photonView.RPC("DocumentLoad", RpcTarget.AllBuffered, fileData);
                }
            }, new string[] { fileExtension });
        }
    }

    [PunRPC]
    void DocumentLoad(byte[] fileData)
    {
        Debug.Log("Picked file: " + fileData.Length + " bytes");

        /*string jpegOutput = Path.Combine("./", "pdf-%d.jpg");
        convertPDF.Convert(path, jpegOutput, 1, 2, "jpeg", 200, 200);*/

        Texture2D texture = null;

        if (fileData != null && fileData.Length > 0)
        {
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            image.texture = texture;
        }
    }
}
