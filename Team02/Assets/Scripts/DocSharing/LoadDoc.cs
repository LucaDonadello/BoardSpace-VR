using UnityEngine;
using System.IO;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class LoadDoc : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform player;
    public Transform cameraTransform;
    public string InteractableTag = "Interactable";  // The tag for interactable objects (e.g., TVs, Lamps)
    public float maxDistance = 10f;  // Maximum distance for raycast
    public LayerMask hitLayers;  // Layers to interact with
    private string fileExtension = "image/*";
    [SerializeField] RawImage image;
    //[SerializeField] PDFViewer pdfViewer;
    void Start()
    {
        // Ensure the LineRenderer is set up
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }

    void Update()
    {
        Vector3 startPosition = player.position;
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
                hitInfo.collider.gameObject.name.Contains("Load"))
            {
                // Get the Light component from the child
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

        // Show the ray with LineRenderer
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}
