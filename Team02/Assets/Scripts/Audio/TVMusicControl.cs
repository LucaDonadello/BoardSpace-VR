using UnityEngine;

public class TVMusicControl : MonoBehaviour
{
    public AudioClip[] musicTracks;  // Array of audio clips for the TV object
    public bool loop = true;  // Whether to loop through the music tracks
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        // Ensure the AudioSource is attached to the TV
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;  // Prevent music from starting automatically
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
}
