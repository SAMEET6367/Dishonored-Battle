using UnityEngine;

public class PlayerSFXWithIntervals : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Ground Check")]
    [Tooltip("Optional — assign the player's jump script so footsteps only play while grounded. Auto-found if left empty.")]
    public jump playerJump;

    [Header("Sound Effects")]
    [Tooltip("Add a few different footstep clips here — one gets picked at random each step instead of always the same one.")]
    public AudioClip[] walkSounds;
    public AudioClip shootSound;
    public AudioClip jumpSound;

    [Header("Sound Intervals (seconds)")]
    public float walkInterval = 0.4f;
    public float shootInterval = 0.2f;
    public float jumpInterval = 0.5f;

    [Header("Variation")]
    [Tooltip("Random pitch range applied to every sound so identical repeats don't sound robotic.")]
    public float minPitch = 0.92f;
    public float maxPitch = 1.08f;

    // Timers for each sound
    private float walkTimer = 0f;
    private float shootTimer = 0f;
    private float jumpTimer = 0f;

    void Start()
    {
        if (playerJump == null)
            playerJump = GetComponent<jump>();
    }

    void Update()
    {
        float delta = Time.deltaTime;

        walkTimer += delta;
        shootTimer += delta;
        jumpTimer += delta;

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool isGrounded = playerJump == null || playerJump.IsGrounded;

        // WALKING — only while actually on the ground, and with a randomly
        // picked clip + pitch each time so it doesn't sound like the exact
        // same sound looping.
        if (isMoving && isGrounded && walkTimer >= walkInterval)
        {
            PlayRandomized(PickRandomClip(walkSounds));
            walkTimer = 0f;
        }

        // SHOOT
        if (Input.GetMouseButtonDown(0) && shootTimer >= shootInterval)
        {
            PlayRandomized(shootSound);
            shootTimer = 0f;
        }

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) && jumpTimer >= jumpInterval)
        {
            PlayRandomized(jumpSound);
            jumpTimer = 0f;
        }
    }

    AudioClip PickRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    void PlayRandomized(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
    }
}