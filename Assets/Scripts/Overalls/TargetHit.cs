using UnityEngine;
using System.Collections;

public class ReplaceOnCollision : MonoBehaviour
{
    public MainObject mainObject;
    public GameObject replacementObject;
    public float animationDuration = 1.5f;

    private bool hasCollided = false; // prevents multiple triggers

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        // 🔴 Disable BoxCollider immediately
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            box.enabled = false;
        }

        // Hide original mesh
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer rend in renderers)
        {
            rend.enabled = false;
        }

        // Spawn local replacement
        SpawnReplacement();

        // Notify main object
        if (mainObject != null)
        {
            mainObject.NotifyExecution();
        }
    }

    void SpawnReplacement()
    {
        if (replacementObject == null) return;

        GameObject obj = Instantiate(replacementObject, transform.position, transform.rotation);

        StartCoroutine(PopScale(obj, animationDuration));
    }

    IEnumerator PopScale(GameObject obj, float duration)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.zero;

        float halfTime = duration / 2f;
        float t = 0f;

        // Grow
        while (t < halfTime)
        {
            t += Time.deltaTime;
            float scale = Mathf.SmoothStep(0f, 1f, t / halfTime);
            obj.transform.localScale = originalScale * scale;
            yield return null;
        }

        t = 0f;

        // Shrink
        while (t < halfTime)
        {
            t += Time.deltaTime;
            float scale = Mathf.SmoothStep(1f, 0f, t / halfTime);
            obj.transform.localScale = originalScale * scale;
            yield return null;
        }

        Destroy(obj);
    }
}