using UnityEngine;
using UnityEngine.Video;

public class YouTubeVideoCanvas : MonoBehaviour
{
    public GameObject videoCanvas;
    public VideoPlayer videoPlayer;

    void Start()
    {
        // set the video player to false
        videoPlayer.Pause();
    }

    public void ToggleVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            videoCanvas.SetActive(true);
            videoPlayer.Play();
            Debug.Log("Video started.");
        }
        else
        {
            videoPlayer.Pause();
            Debug.Log("Video paused.");
        }
    }
}