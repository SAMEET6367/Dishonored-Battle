using UnityEngine;

public class Death : MonoBehaviour
{
    public MonoBehaviour[] scriptsToDisable;

    [Header("Scene Switch")]
    public SimpleSceneSwitch sceneSwitch;

    private bool isDead = false;

    public void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died!");

        // Disable scripts
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        // Stop physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 🔥 LOAD NEXT SCENE
        if (sceneSwitch != null)
        {
            sceneSwitch.LoadNextScene();
        }
        else
        {
            Debug.LogError("SceneSwitch not assigned in Inspector!");
        }
    }
}