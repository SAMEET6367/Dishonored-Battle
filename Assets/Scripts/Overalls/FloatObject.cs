using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    public float height = 1.5f; // how far it moves up/down
    public float speed = 2f;    // how fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}