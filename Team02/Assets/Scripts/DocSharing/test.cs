using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Networking;

public class test : MonoBehaviour
{
    private string pdf = "application/pdf";
    [SerializeField] TMP_Text text;
    [SerializeField] RawImage image;
    private ConvertPDF convertPDF;
    //[SerializeField] PDFViewer pdfViewer;
    void Start() {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        convertPDF = new ConvertPDF();
        image = GetComponent<RawImage>();
    }
    void Update()
    {
        if(Input.GetKeyDown("g") || Input.GetButtonDown("js2")){
            // Don't attempt to import/export files if the file picker is already open
            if( NativeFilePicker.IsFilePickerBusy() )
                return;
            NativeFilePicker.PickFile( ( path ) =>
			{
				if( path == null )
					Debug.Log( "Operation cancelled" );
				else{
					Debug.Log( "Picked file: " + path );
                    text.text = path;

                    string jpegOutput = Path.Combine("./", "pdf-%d.jpg");

                    convertPDF.Convert(path, jpegOutput, 1, 2, "jpeg", 200, 200);

                    LoadPDF(jpegOutput);
                }
			}, new string[] { pdf } );
        }
    }
    IEnumerator LoadPDF(string filePath){
        var www = UnityWebRequestTexture.GetTexture(filePath);
        yield return www.SendWebRequest();
        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        image.texture = texture;
    }
}
