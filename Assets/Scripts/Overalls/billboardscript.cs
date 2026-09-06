using UnityEngine;

public class billboardscript : MonoBehaviour
{
    public Transform cam;
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
