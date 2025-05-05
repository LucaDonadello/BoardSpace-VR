using UnityEngine;

public class ManualBallReset : MonoBehaviour
{
    public float resetCooldown = 0.5f; // seconds to wait after throw
    private float lastThrowTime = -Mathf.Infinity;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        //don't throw an error when first starting
        if(ButtonMapping.Instance == null)
            return;

        //Wait after last throw so not throwing and resetting position at the same time
        if ((Input.GetKeyDown(KeyCode.R) || ButtonMapping.Instance.GetActionDown("B")) && Time.time > lastThrowTime + resetCooldown)
            ResetBall();
    }

    public void RegisterThrow()
    {
        lastThrowTime = Time.time;
    }

    private void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        Debug.Log("Ball manually reset.");
    }
}
