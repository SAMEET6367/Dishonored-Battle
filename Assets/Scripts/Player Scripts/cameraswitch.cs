using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private bool usingCamera1 = true;

    void Start()
    {
        // Enable the first camera, disable the second
        camera1.enabled = true;
        camera2.enabled = false;
    }

    void Update()
    {
        // Press V to switch cameras
        if (Keyboard.current != null && Keyboard.current.vKey != null)
        {
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                usingCamera1 = !usingCamera1;

                camera1.enabled = usingCamera1;
                camera2.enabled = !usingCamera1;
            }
        }
    }
}