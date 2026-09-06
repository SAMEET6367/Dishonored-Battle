using UnityEngine;

public class PlayMusicOnTrigger : MonoBehaviour
{
    public AudioSource musicSource; // Drag your AudioSource here
    public AudioClip newMusic;      // The music to play

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered
        if (!hasPlayed && other.CompareTag("Player"))
        {
            musicSource.clip = newMusic;
            musicSource.Play();
            hasPlayed = true; // ensures it only plays once
        }
    }
}