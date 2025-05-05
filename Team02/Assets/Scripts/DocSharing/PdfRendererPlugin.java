package com.yourcompany.pdfplugin;

import android.graphics.Bitmap;
import android.graphics.pdf.PdfRenderer;
import android.os.ParcelFileDescriptor;
import android.util.Base64;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;

public class PdfRendererPlugin {

    // Unity calls this method with a full path to the PDF
    public static String renderPdfToBase64Image(String pdfPath) {
        try {
            File file = new File(pdfPath);
            ParcelFileDescriptor fd = ParcelFileDescriptor.open(file, ParcelFileDescriptor.MODE_READ_ONLY);

            PdfRenderer renderer = new PdfRenderer(fd);
            PdfRenderer.Page page = renderer.openPage(0);

            int width = page.getWidth();
            int height = page.getHeight();
            Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);

            page.render(bitmap, null, null, PdfRenderer.Page.RENDER_MODE_FOR_DISPLAY);

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
            byte[] imageBytes = stream.toByteArray();

            page.close();
            renderer.close();
            fd.close();

            // Encode image as Base64 to send to Unity
            return Base64.encodeToString(imageBytes, Base64.DEFAULT);

        } catch (IOException e) {
            e.printStackTrace();
            return "";
        }
    }
    public static String renderPdfPageToBase64Image(String pdfPath, int pageIndex) {
        try {
            File file = new File(pdfPath);
            ParcelFileDescriptor fd = ParcelFileDescriptor.open(file, ParcelFileDescriptor.MODE_READ_ONLY);
    
            PdfRenderer renderer = new PdfRenderer(fd);
            if (pageIndex < 0 || pageIndex >= renderer.getPageCount()) {
                renderer.close();
                fd.close();
                return "";
            }
    
            PdfRenderer.Page page = renderer.openPage(pageIndex);
    
            int width = page.getWidth();
            int height = page.getHeight();
            Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
    
            page.render(bitmap, null, null, PdfRenderer.Page.RENDER_MODE_FOR_DISPLAY);
    
            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
            byte[] imageBytes = stream.toByteArray();
    
            page.close();
            renderer.close();
            fd.close();
    
            return Base64.encodeToString(imageBytes, Base64.DEFAULT);
    
        } catch (IOException e) {
            e.printStackTrace();
            return "";
        }
    }
    public static int getPdfPageCount(String pdfPath) {
        try {
            File file = new File(pdfPath);
            ParcelFileDescriptor fd = ParcelFileDescriptor.open(file, ParcelFileDescriptor.MODE_READ_ONLY);
            PdfRenderer renderer = new PdfRenderer(fd);
            int count = renderer.getPageCount();
            renderer.close();
            fd.close();
            return count;
        } catch (IOException e) {
            e.printStackTrace();
            return 0;
        }
    }
            
    public static String renderAllPdfPagesToBase64Images(String pdfPath) {
        JSONArray base64List = new JSONArray();
    
        try {
            File file = new File(pdfPath);
            ParcelFileDescriptor fd = ParcelFileDescriptor.open(file, ParcelFileDescriptor.MODE_READ_ONLY);
            PdfRenderer renderer = new PdfRenderer(fd);
    
            int pageCount = renderer.getPageCount();
    
            for (int i = 0; i < pageCount; i++) {
                PdfRenderer.Page page = renderer.openPage(i);
    
                int width = page.getWidth();
                int height = page.getHeight();
                Bitmap bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888);
    
                page.render(bitmap, null, null, PdfRenderer.Page.RENDER_MODE_FOR_DISPLAY);
    
                ByteArrayOutputStream stream = new ByteArrayOutputStream();
                bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
                byte[] imageBytes = stream.toByteArray();
    
                String base64 = Base64.encodeToString(imageBytes, Base64.NO_WRAP);
                base64List.put(base64);
    
                page.close();
            }
    
            renderer.close();
            fd.close();
    
        } catch (IOException e) {
            e.printStackTrace();
        }
    
        return base64List.toString(); // Return JSON array string
    }
    
}
