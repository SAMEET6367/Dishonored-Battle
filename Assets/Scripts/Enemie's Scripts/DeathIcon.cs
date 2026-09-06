using UnityEngine;

public class DeathIcon : MonoBehaviour
{
    [Header("Scale")]
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;
    public float scaleDuration = 0.3f;

    [Header("Movement")]
    public float riseSpeed = 1f;

    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(0f, 180f, 0f);

    private float timer = 0f;

    private void Start()
    {
        transform.localScale = startScale;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Scale up
        float t = Mathf.Clamp01(timer / scaleDuration);
        transform.localScale = Vector3.Lerp(startScale, endScale, t);

        // Move upward
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Rotate
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}