using UnityEngine;

public class BounceSound : MonoBehaviour
{
    public AudioClip bounceSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PingPongTable"))
        {
            if (bounceSound != null && audioSource != null)
                audioSource.PlayOneShot(bounceSound);
        }
    }
}
