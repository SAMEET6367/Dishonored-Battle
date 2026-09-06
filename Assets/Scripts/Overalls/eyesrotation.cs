using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    public float angle = 30f;   // maximum swing angle
    public float speed = 1f;    // swing speed

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float swing = Mathf.Sin(Time.time * speed) * angle;
        transform.rotation = startRotation * Quaternion.Euler(swing, 0f, 0f);
    }
}