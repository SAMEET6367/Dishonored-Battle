using UnityEngine;

public class PlayerSFXWithIntervals : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip walkSound;
    public AudioClip shootSound;
    public AudioClip jumpSound;

    [Header("Sound Intervals (seconds)")]
    public float walkInterval = 0.3f;
    public float shootInterval = 0.2f;
    public float jumpInterval = 0.5f;

    // Timers for each sound
    private float walkTimer = 0f;
    private float shootTimer = 0f;
    private float jumpTimer = 0f;

    void Update()
    {
        float delta = Time.deltaTime;

        // Update timers
        walkTimer += delta;
        shootTimer += delta;
        jumpTimer += delta;

        // WALKING
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
             Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && walkTimer >= walkInterval)
        {
            audioSource.PlayOneShot(walkSound);
            walkTimer = 0f;
        }

        // SHOOT
        if (Input.GetMouseButtonDown(0) && shootTimer >= shootInterval)
        {
            audioSource.PlayOneShot(shootSound);
            shootTimer = 0f;
        }

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) && jumpTimer >= jumpInterval)
        {
            audioSource.PlayOneShot(jumpSound);
            jumpTimer = 0f;
        }
    }
}