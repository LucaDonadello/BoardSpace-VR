using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Transform rayOrigin;
    public string targetTag1 = "Door";
    public string targetTag2 = "Interactive";
    public float maxDistance = 10;

    private Outline lastOutlinedObject = null;

    void Update()
    {

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(rayOrigin.position, Camera.main.transform.forward, out hitInfo, maxDistance);

        if (hit)
        {
            Collider hitCollider = hitInfo.collider;

            if (hitCollider != null && (hitCollider.tag.Contains(targetTag1) || hitCollider.tag.Contains(targetTag2)))
            {
                Outline outline = hitCollider.GetComponent<Outline>();
                Debug.Log($"Hit Object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");

                if (lastOutlinedObject != null && lastOutlinedObject != outline)
                {
                    lastOutlinedObject.enabled = false;
                }


                if (outline != null)
                {
                    outline.enabled = true;
                    lastOutlinedObject = outline;
                }
            }

            else
            {
                if (lastOutlinedObject != null)
                {
                    lastOutlinedObject.enabled = false;
                    lastOutlinedObject = null;
                }
            }
        }

        else
        {
            if (lastOutlinedObject != null)
            {
                lastOutlinedObject.enabled = false;
                lastOutlinedObject = null;
            }
        }
    }
}
