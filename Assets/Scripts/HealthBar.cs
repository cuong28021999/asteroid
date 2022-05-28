using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float speedSlide = 60f;
    private int value;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        value = health;
    }
    public void SetHealth(int health)
    {
        value = health;
    }

    public void SetValue(int health)
    {
        value = health;
        slider.value = value;
    }

    private void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, value, speedSlide * Time.deltaTime);
    }
}
