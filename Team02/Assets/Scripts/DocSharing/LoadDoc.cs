using UnityEngine;
using System.IO;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class LoadDoc : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";  // The tag for interactable objects (e.g., TVs, Lamps)
    public float maxDistance = 10f;  // Maximum distance for raycast
    public LayerMask hitLayers;  // Layers to interact with
    private string fileExtension = "image/*";
    private PlayerData playerData;  // Reference to PlayerData script
    [SerializeField] RawImage image;
    //[SerializeField] PDFViewer pdfViewer;
    void Start()
    {
        playerData = player.GetComponent<PlayerData>();  // Get PlayerData component from the player
        maxDistance = playerData.playerRayLength;  // Set max distance from PlayerData
        // Ensure the LineRenderer is set up
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        image = FindFirstObjectByType<RawImage>();
    }

    void Update()
    {
        float maxDistance = playerData.playerRayLength;
        Vector3 startPosition = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        Vector3 endPosition = startPosition + direction * maxDistance;

        RaycastHit hitInfo;
        Debug.Log("Running");
        // Perform the raycast
        if (Physics.Raycast(startPosition, direction, out hitInfo, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            endPosition = hitInfo.point;
            Debug.Log("Hitting");
            // Check for input and correct tag/name
            // Press Y on the keyboard or X on the controller to interact with the lamp
            if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) &&
                hitInfo.collider != null &&
                hitInfo.collider.CompareTag(InteractableTag) &&
                hitInfo.collider.gameObject.name.Contains("Load"))
            {
                Debug.Log("load");
                if( NativeFilePicker.IsFilePickerBusy() )
                return;
                NativeFilePicker.PickFile( ( path ) =>
                {
                    if( path == null )
                        Debug.Log( "Operation cancelled" );
                    else{
                        Debug.Log( "Picked file: " + path );

                        /*string jpegOutput = Path.Combine("./", "pdf-%d.jpg");

                        convertPDF.Convert(path, jpegOutput, 1, 2, "jpeg", 200, 200);*/

                        Texture2D texture = null;
                        byte[] fileData;

                        if(File.Exists(path)){
                            fileData = File.ReadAllBytes(path);
                            texture = new Texture2D(2,2);
                            texture.LoadImage(fileData);
                            image.texture = texture;
                        }
                    }
                }, new string[] { fileExtension } );
            }
        }
    }
}
