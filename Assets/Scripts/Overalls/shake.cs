using UnityEngine;

public class PlatformShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeHeight = 0.05f;   // How much it moves up/down
    public float shakeSpeed = 8f;       // How fast it vibrates

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * shakeSpeed) * shakeHeight;

        transform.position = startPosition + new Vector3(0f, offset, 0f);
    }
}