using UnityEngine;
using System.Collections.Generic;

public class OutlineEffect : MonoBehaviour
{
    [Header("Raycast Settings")]

    public float maxDistance = 10f;
    public LineRenderer lineRenderer;
    public Transform rayOrigin;
    public static Transform grabPosition;
    
    [Header("Tags to Outline (partial match allowed)")]
    public List<string> targetTags = new List<string> { "Door", "Interactable" };
    private Outline lastOutlinedObject = null;
    private GameObject grabPoint;
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;

        //create grab point 
        grabPoint = new GameObject("GrabPoint");
        grabPoint.transform.SetParent(rayOrigin);
        grabPoint.transform.localPosition = new Vector3(0f, -0.02f, 4f);
        grabPoint.transform.localRotation = Quaternion.identity;
        grabPosition = grabPoint.transform;
    }

    void Update()
    {
        grabPoint = new GameObject("GrabPoint");
        grabPoint.transform.SetParent(rayOrigin);
        grabPoint.transform.localPosition = new Vector3(0f, -0.02f, 4f);
        grabPoint.transform.localRotation = Quaternion.identity;
        grabPosition = grabPoint.transform;
        
        //move raycast up so it's more visible
        Vector3 offsetOrigin = rayOrigin.position - rayOrigin.up * 0.02f;

        Vector3 direction = rayOrigin.forward;
        Ray ray = new Ray(offsetOrigin, direction);

        RaycastHit hit;
        Vector3 endPoint = offsetOrigin + direction * maxDistance;

        lineRenderer.SetPosition(0, ray.origin);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            Debug.DrawRay(offsetOrigin, direction * hit.distance, Color.red);
            Collider hitCollider = hit.collider; //changed hitInfo to hit
            string tag = hitCollider.tag;

            lineRenderer.SetPosition(1, hit.point);

            // Check if tag contains any of the defined target tags
            bool isMatchingTag = targetTags.Exists(t => tag.Contains(t));
            if (isMatchingTag)
            {
                Outline outline = hitCollider.GetComponent<Outline>();
                if (outline == null)
                    return;

                if (lastOutlinedObject != null && lastOutlinedObject != outline)
                {
                    lastOutlinedObject.enabled = false;
                }

                outline.enabled = true;
                lastOutlinedObject = outline;
                return;
            }
        }
        else //Ray didn't hit anything
        {
            lineRenderer.SetPosition(1, endPoint);
        }

        // No valid object hit or tag doesn't match
        if (lastOutlinedObject != null)
        {
            lastOutlinedObject.enabled = false;
            lastOutlinedObject = null;
        }
    }
}
