using UnityEngine;

public class Raycast_hit : MonoBehaviour
{
    //go to build profiles, make sure new scene is added to list when we want to switch
    //UnityEngine scene management
    //Collision detection: OnCollisionEnter, OnTriggerEnter, make sure they're rigid bodies, 
    //make sure IsTrigger is off, and isKinematic too?
    public float rayLength = 10f;
    public LineRenderer lineRenderer;
    public Transform headCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offsetOrigin = headCamera.position + headCamera.forward * 0.1f - headCamera.up * 0.1f; //add downward offset

        Vector3 direction = headCamera.forward;
        Ray ray = new Ray(offsetOrigin, direction);

        RaycastHit hit;
        Vector3 endPoint = offsetOrigin + direction * rayLength;

        lineRenderer.SetPosition(0, ray.origin);

        int layerMask = ~LayerMask.GetMask("IgnoreRay");
        if(Physics.Raycast(ray, out hit, rayLength, layerMask)) //ray hits something
        {
            //Debug.Log("Raycast hit: " + hit.collider.name);
            Debug.DrawRay(offsetOrigin, direction * hit.distance, Color.red);
            //stop raycast with thing it colides with
            lineRenderer.SetPosition(1, hit.point);
            //SendMessage("hoverin");
        }
        else
        {
            Debug.DrawRay(offsetOrigin, direction * rayLength, Color.green);
            lineRenderer.SetPosition(1, endPoint);
            //SendMessage("hoverout");
        }

    }
}
