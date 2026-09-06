using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip newMusic;

    [Header("Player Layer")]
    public LayerMask whatIsPlayer;

    [Header("Settings")]
    public bool playOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is on the player layer
        if (((1 << other.gameObject.layer) & whatIsPlayer) == 0)
            return;

        if (playOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (musicSource != null && newMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }
}