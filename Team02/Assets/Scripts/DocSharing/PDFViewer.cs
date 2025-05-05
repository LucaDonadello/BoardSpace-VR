using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PDFViewer : MonoBehaviour
{
    AndroidJavaClass pdfClass;
    string pdfPath;
    public RawImage display;
    int pageCount;
    int currentPage = 0;

    void Start()
    {
        pdfClass = new AndroidJavaClass("com.example.pdfplugin.PdfRendererPlugin");
    }

    public void LoadPDF(string path)
    {
        pdfPath = path;
        pageCount = pdfClass.CallStatic<int>("getPdfPageCount", pdfPath);
        Debug.Log("PAGE COUNT: " + pageCount);
        currentPage = 0;
        LoadPage(currentPage);
    }

    public void LoadPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageCount) {
            Debug.Log("FAIL");
            return;
        }

        string base64 = pdfClass.CallStatic<string>("renderPdfPageToBase64Image", pdfPath, pageIndex);
        if (!string.IsNullOrEmpty(base64))
        {
            Debug.Log("LOADING");
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            display.texture = tex;
        }
    }

    public void NextPage()
    {
        if (currentPage < pageCount - 1)
        {
            currentPage++;
            LoadPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            LoadPage(currentPage);
        }
    }
}
