using UnityEngine;

public class KillPlayerOnTouch : MonoBehaviour
{
    public int damage = 9999; // enough to instantly kill

    [Header("Audio")]
    public AudioSource audioSource;   // assign in Inspector
    public AudioClip hitSound;        // sound when collision happens
    public float soundInterval = 0.2f; // prevents spam
    private float soundTimer = 0f;

    void Update()
    {
        soundTimer += Time.deltaTime;
    }

    // Use this for normal colliders (Is Trigger = OFF)
    private void OnCollisionEnter(Collision collision)
    {
        PlaySound();
        TryKillPlayer(collision.gameObject);
    }

    // Trigger support
    private void OnTriggerEnter(Collider other)
    {
        PlaySound();
        TryKillPlayer(other.gameObject);
    }

    // Play sound with interval check
    void PlaySound()
    {
        if (audioSource != null && hitSound != null && soundTimer >= soundInterval)
        {
            audioSource.PlayOneShot(hitSound);
            soundTimer = 0f;
        }
    }

    // Common kill function
    private void TryKillPlayer(GameObject obj)
    {
        AttributesManager playerAttributes = obj.GetComponent<AttributesManager>();
        if (playerAttributes != null && !playerAttributes.IsDead())
        {
            playerAttributes.TakeDamage(damage);
        }
    }
}