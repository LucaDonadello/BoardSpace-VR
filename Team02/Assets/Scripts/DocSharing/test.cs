using UnityEngine;
using System.Collections;
using TMPro;

public class test : MonoBehaviour
{
    private string pdf = "application/pdf";
    [SerializeField] TMP_Text text;
    [SerializeField] PDFViewer pdfViewer;
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
                    pdfViewer.LoadPDF(path);
                }
			}, new string[] { pdf } );
        }
    }
}
