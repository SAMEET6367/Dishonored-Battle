using UnityEngine;

public class RotateXLoop : MonoBehaviour
{
    public float loopDuration = 2f; // time (in seconds) for ONE full rotation

    void Update()
    {
        float rotationPerSecond = 360f / loopDuration;
        transform.Rotate(rotationPerSecond * Time.deltaTime, 0f, 0f);
    }
}