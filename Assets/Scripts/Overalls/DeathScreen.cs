using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // =========================
    // LOAD PREVIOUS SCENE
    // =========================
    public void LoadPreviousScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex - 1);
    }

    // =========================
    // LOAD SPECIFIC SCENE
    // =========================
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(0);
    }
}