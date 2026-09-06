using UnityEngine;

public class SlideZOnce : MonoBehaviour
{
    public float targetZ = 10f;   // destination Z position
    public float speed = 2f;      // movement speed

    void Update()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}