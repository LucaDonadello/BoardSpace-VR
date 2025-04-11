using UnityEngine;

public class TVMusicControl : MonoBehaviour
{
    public AudioClip[] musicTracks;  // Array of audio clips for the TV object
    public bool loop = true;  // Whether to loop through the music tracks
    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private bool isPlaying = false;
    private bool onObject = false;
    //public Outline outline;

    void Start()
    {
        // Ensure the AudioSource is attached to the TV
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;  // Prevent music from starting automatically
        //outline = GetComponent<Outline>();
        //outline.enabled = false;
    }

    void Update()
    {
         // Check if the hit object has the tag "TV"
        if ((Input.GetKeyDown(KeyCode.Y) || Input.GetButtonDown("js2")) && onObject)
        {
            Debug.Log("Keypress detected");
            if (!isPlaying)
            {
                PlayNextTrack();  // Start playing the music on the TV
                isPlaying = true;
                Debug.Log("Playing music on TV.");
            }
            else
            {
                StopMusic();  // Stop music on the TV
                isPlaying = false;
                Debug.Log("Stopped music on TV.");
            }
        }       

    }

    public void PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;

        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();

        currentTrackIndex++;

        if (currentTrackIndex >= musicTracks.Length)
        {
            currentTrackIndex = loop ? 0 : musicTracks.Length - 1;
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();  // Stop the currently playing music
    }

    public void hoverin()
    {
        Debug.Log("Inside tv hoverin()");
        onObject = true;
        //outline.enabled = true;
        //Debug.Log("outline.enabled: " + outline.enabled);
    }

    public void hoverout()
    {
        Debug.Log("Inside tv hoverout()");
        onObject = false;
        //outline.enabled = false;
        //Debug.Log("outline.enabled: " + outline.enabled);
    }
}
