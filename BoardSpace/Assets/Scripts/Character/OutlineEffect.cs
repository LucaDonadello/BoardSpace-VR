using UnityEngine;
using System.Collections.Generic;

public class OutlineEffect : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Transform rayOrigin;
    public float maxDistance = 10f;

    [Header("Tags to Outline (partial match allowed)")]
    public List<string> targetTags = new List<string> { "Door", "Interactable", "Grabbable" };

    private Outline lastOutlinedObject = null;

    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, Camera.main.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
        {
            Collider hitCollider = hitInfo.collider;
            string tag = hitCollider.tag;

            Debug.Log($"Hit Object: {hitCollider.name}, Tag: {tag}");

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

        // No valid object hit or tag doesn't match
        if (lastOutlinedObject != null)
        {
            lastOutlinedObject.enabled = false;
            lastOutlinedObject = null;
        }
    }
}
