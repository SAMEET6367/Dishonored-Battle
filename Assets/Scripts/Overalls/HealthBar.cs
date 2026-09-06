using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;       // Gradient for health color
    public Image fillImage;         // The fill image of the slider

    // Initialize slider value and color
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        // Set color to full health (1f)
        if (fillImage != null && gradient != null)
            fillImage.color = gradient.Evaluate(1f);
    }

    // Update slider value and gradient color based on current health
    public void SetHealth(int health)
    {
        slider.value = health;

        if (fillImage != null && gradient != null)
        {
            float normalized = slider.value / slider.maxValue;
            fillImage.color = gradient.Evaluate(normalized);
        }
    }
}
