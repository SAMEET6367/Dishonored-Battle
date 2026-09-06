using UnityEngine;
using UnityEngine.SceneManagement;

public class MainObject : MonoBehaviour
{
    [Header("Hit Count Settings")]
    public int executionsToDisappear = 3;

    private int executionCount = 0;
    private bool replaced = false;

    // ===== EXPLOSION SETTINGS =====
    [Header("Death Explosion")]
    public GameObject[] deathPieces;
    public int piecesToSpawn = 10;
    public float explosionForce = 300f;
    public float explosionRadius = 3f;
    public float pieceLifetime = 5f;

    // ===== SCENE SETTINGS =====
    [Header("Scene Settings")]
    public int sceneToLoad = 0; // set this in Inspector

    // Called by ReplaceOnCollision
    public void NotifyExecution()
    {
        if (replaced) return;

        executionCount++;

        if (executionCount >= executionsToDisappear)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        replaced = true;

        // Hide original mesh
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer rend in renderers)
        {
            rend.enabled = false;
        }

        // Disable all other scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }

        // 💥 EXPLOSION LOGIC
        if (deathPieces != null && deathPieces.Length > 0)
        {
            for (int i = 0; i < piecesToSpawn; i++)
            {
                GameObject prefab = deathPieces[Random.Range(0, deathPieces.Length)];

                GameObject piece = Instantiate(
                    prefab,
                    transform.position,
                    prefab.transform.rotation
                );

                Rigidbody rb = piece.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(
                        explosionForce,
                        transform.position,
                        explosionRadius
                    );
                }

                Destroy(piece, pieceLifetime);
            }
        }

        // 🔥 LOAD SCENE AFTER BOSS DIES
        SceneManager.LoadScene(8);

        // Optional: destroy the main object
        Destroy(gameObject, 0.1f);
    }
}