using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float resetHeight = -2.6f; //if out of height bounds, teleport back
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogWarning("ResetPosition: No Rigidbody found on object.");
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < resetHeight)
            ResetObject();
    }

    private void ResetObject()
    {
        //Stop movement and transport back to original position
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
