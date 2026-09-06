using UnityEngine;

public class ActivateScriptsOnTrigger : MonoBehaviour
{
    [Header("Scripts to Enable")]
    public MonoBehaviour[] scriptsToEnable; // drag scripts here

    [Header("Optional")]
    public bool disableAfterTrigger = true; // disables this trigger after use

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (MonoBehaviour script in scriptsToEnable)
        {
            if (script != null)
                script.enabled = true;
        }

        if (disableAfterTrigger)
            gameObject.SetActive(false);
    }
}