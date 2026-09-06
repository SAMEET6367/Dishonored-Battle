using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnDeath : MonoBehaviour
{
    [Header("References")]
    public AttributesManager attributesManager;

    [Header("Scene Settings")]
    [Tooltip("Build Index of the scene to load.")]
    public int sceneNumber = 0;

    private bool sceneLoaded = false;

    private void Update()
    {
        if (sceneLoaded || attributesManager == null)
            return;

        // Load the scene when health reaches 0 or below
        if (attributesManager.health <= 0)
        {
            sceneLoaded = true;

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
}