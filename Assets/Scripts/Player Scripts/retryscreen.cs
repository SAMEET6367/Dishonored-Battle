using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneSwitch : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentIndex + 1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}