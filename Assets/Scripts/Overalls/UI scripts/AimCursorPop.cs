using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonPop : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Startup")]
    public bool disabledAtLaunch = false;

    [Header("Pop Animation")]
    public float scaleMultiplier = 1.2f;
    public float animationTime = 0.15f;

    [Header("Click Interval")]
    public float clickInterval = 0.5f;

    private Vector3 originalScale;

    private bool isAnimating;
    private bool canClick = true;
    private bool isVisible;

    void Start()
    {
        originalScale = targetImage.rectTransform.localScale;

        // Set starting visibility
        isVisible = !disabledAtLaunch;

        targetImage.enabled = isVisible;
    }

    void Update()
    {
        // Toggle image with V
        if (Input.GetKeyDown(KeyCode.V))
        {
            isVisible = !isVisible;

            // Enable/Disable ONLY the image component
            targetImage.enabled = isVisible;
        }

        // LMB animation
        if (Input.GetMouseButtonDown(0) &&
            !isAnimating &&
            canClick &&
            isVisible)
        {
            StartCoroutine(PopAnimation());
            StartCoroutine(ClickCooldown());
        }
    }

    IEnumerator PopAnimation()
    {
        isAnimating = true;

        Vector3 targetScale = originalScale * scaleMultiplier;

        float timer = 0f;

        // Grow
        while (timer < animationTime)
        {
            timer += Time.deltaTime;

            targetImage.rectTransform.localScale =
                Vector3.Lerp(originalScale, targetScale, timer / animationTime);

            yield return null;
        }

        timer = 0f;

        // Shrink
        while (timer < animationTime)
        {
            timer += Time.deltaTime;

            targetImage.rectTransform.localScale =
                Vector3.Lerp(targetScale, originalScale, timer / animationTime);

            yield return null;
        }

        targetImage.rectTransform.localScale = originalScale;

        isAnimating = false;
    }

    IEnumerator ClickCooldown()
    {
        canClick = false;

        yield return new WaitForSeconds(clickInterval);

        canClick = true;
    }
}