using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Transform rayOrigin;
    public string targetTag = "Door";
    public float maxDistance = 10;

    private Outline lastOutlinedObject = null;

    void Update()
    {

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(rayOrigin.position, Camera.main.transform.forward, out hitInfo, maxDistance);

        if (hit)
        {
            Collider hitCollider = hitInfo.collider;

            if (hitCollider != null && hitCollider.tag.Contains(targetTag))
            {
                Outline outline = hitCollider.GetComponent<Outline>();


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
