using UnityEngine;

public class FogFadeTrigger : MonoBehaviour
{
    public float fadeDuration = 3f; // time in seconds to fully fade
    private bool playerInside = false;
    private float initialFogDensity;

    void Start()
    {
        initialFogDensity = RenderSettings.fogDensity; // store starting fog density
    }

    void Update()
    {
        if (playerInside)
        {
            // Reduce fog density smoothly over time
            RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, 0f, (initialFogDensity / fadeDuration) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
}