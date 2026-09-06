using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Build Index of the scene to load.")]
    public int sceneNumber = 0;

    // This function is called by the UI Button
    public void LoadScene()
    {
        if (sceneNumber >= 0 && sceneNumber < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneNumber);
        }
        else
        {
            Debug.LogError("Scene " + sceneNumber + " is not in Build Settings!");
        }
    }
}