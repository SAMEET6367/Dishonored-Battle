using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    public AudioSource musicSource; // Drag your AudioSource here
    public AudioClip bossMusic;     // Boss music clip
    public AudioClip normalMusic;   // Optional: normal music clip

    private bool musicSwitched = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if player enters the trigger
        if (!musicSwitched && other.CompareTag("Player"))
        {
            musicSource.clip = bossMusic;
            musicSource.Play();
            musicSwitched = true; // prevent switching again
        }
    }

    // Optional: switch back when leaving trigger
    private void OnTriggerExit(Collider other)
    {
        if (musicSwitched && other.CompareTag("Player"))
        {
            musicSource.clip = normalMusic;
            musicSource.Play();
            musicSwitched = false;
        }
    }
}