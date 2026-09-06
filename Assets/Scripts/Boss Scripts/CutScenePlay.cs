using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoTrigger : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    public Canvas videoCanvas;

    [Header("Player")]
    public LayerMask whatIsPlayer;

    [Header("Settings")]
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void Start()
    {
        if (videoCanvas != null)
            videoCanvas.gameObject.SetActive(false);

        if (videoPlayer != null)
            videoPlayer.loopPointReached += VideoFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsPlayer) == 0)
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        hasPlayed = true;

        StartCoroutine(PlayVideo());
    }

    IEnumerator PlayVideo()
    {
        // Pause the game
        Time.timeScale = 0f;

        // Show the video
        videoCanvas.gameObject.SetActive(true);

        // Video keeps playing even when Time.timeScale = 0
        videoPlayer.Play();

        yield return null;
    }

    private void VideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();

        if (videoCanvas != null)
            videoCanvas.gameObject.SetActive(false);

        // Resume the game
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= VideoFinished;
    }
}