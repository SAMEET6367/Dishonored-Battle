using UnityEngine;

public class RotateZLoop : MonoBehaviour
{
    public float speed = 100f; // rotation speed

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}