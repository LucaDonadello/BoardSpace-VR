using UnityEngine;

public class CupTrigger : MonoBehaviour
{
    public GameObject cupToRemove;     
    public GameObject ballToRespawn;   
    private Vector3 ballOriginalPosition;
    private Quaternion ballOriginalRotation;
    private Rigidbody ballRb;

    void Start()
    {
        if (ballToRespawn != null)
        {
            ballOriginalPosition = ballToRespawn.transform.position;
            ballOriginalRotation = ballToRespawn.transform.rotation;
            ballRb = ballToRespawn.GetComponent<Rigidbody>();
        }
        else
            Debug.Log("No ball detected");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.StartsWith("SM_ping_pong_ball"))
        {
            Debug.Log("Ball entered cup!");

            Destroy(cupToRemove);

            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballToRespawn.transform.position = ballOriginalPosition;
            ballToRespawn.transform.rotation = ballOriginalRotation;
        }
    }
}
