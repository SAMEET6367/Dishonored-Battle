using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneOnBackspace : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
        }

        // Force mouse to stay usable
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}