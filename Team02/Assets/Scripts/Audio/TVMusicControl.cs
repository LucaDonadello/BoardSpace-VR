using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(AudioSource), typeof(PhotonView))]
public class TVMusicControl : MonoBehaviourPun
{
    [Header("Tracks & Looping")]
    public AudioClip[] musicTracks;
    public bool loop = true;

    [Header("3D Attenuation")]
    public float minDistance = 1f;
    public float maxDistance = 3f;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Awake()
    {
        // Ensure the AudioSource is attached to the TV
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;  // Prevent music from starting automatically

        // 3D sound settings
        audioSource.spatialBlend = 1f;                                // full 3D
        audioSource.rolloffMode  = AudioRolloffMode.Logarithmic;
        audioSource.minDistance  = minDistance;
        audioSource.maxDistance  = maxDistance;
    }

    public void PlayNextTrack() => photonView.RPC(nameof(RPC_PlayNextTrack), RpcTarget.AllBuffered);

    public void StopMusic() => photonView.RPC(nameof(RPC_StopMusic), RpcTarget.AllBuffered);

    [PunRPC]
    void RPC_PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;

        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();

        // advance index
        currentTrackIndex++;
        if (currentTrackIndex >= musicTracks.Length)
            currentTrackIndex = loop ? 0 : musicTracks.Length - 1;
    }

    [PunRPC]
    void RPC_StopMusic()
    {
        audioSource.Stop();
    }
}
