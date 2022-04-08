using UnityEngine.UI;
using UnityEngine;

public class BoostingBar : MonoBehaviour
{
    public Slider slider;
    public float speedSlide = 60f;
    private float value;

    public void SetMaxMana(float mana)
    {
        slider.maxValue = mana;
    }
    public void SetMana(float mana)
    {
        value = mana;
    }

    private void Update()
    {
        slider.value = Mathf.MoveTowards(slider.value, value, speedSlide * Time.deltaTime * 4f);
    }
}
