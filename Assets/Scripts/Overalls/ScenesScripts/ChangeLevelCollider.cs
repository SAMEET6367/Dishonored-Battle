using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Build Index of the scene to load.")]
    public int sceneNumber = 0;

    [Header("Player Layer")]
    [Tooltip("Assign the WhatIsPlayer layer here.")]
    public LayerMask whatIsPlayer;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is on the WhatIsPlayer layer
        if (((1 << other.gameObject.layer) & whatIsPlayer) != 0)
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
}