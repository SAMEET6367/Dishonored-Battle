using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 3f;

    private Vector3 startPos;
    private bool movingUp = true;

    [Header("Audio")]
    public AudioSource audioSource;   // assign in Inspector
    public AudioClip moveSound;       // looping movement sound

    void Start()
    {
        startPos = transform.position;

        // Setup looping sound
        if (audioSource != null && moveSound != null)
        {
            audioSource.clip = moveSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (movingUp)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;

            if (transform.position.y >= startPos.y + distance)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;

            if (transform.position.y <= startPos.y)
            {
                movingUp = true;
            }
        }
    }
}