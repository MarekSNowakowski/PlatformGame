using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider shieldSlider;

    public void InitializeSliders(float healthMaxValue, float shieldMaxValue)
    {
        healthSlider.maxValue = healthMaxValue;
        healthSlider.value = healthMaxValue;
        shieldSlider.maxValue = shieldMaxValue;
        shieldSlider.value = shieldMaxValue;
    }

    public void UpdateHealth(float newValue)
    {
        healthSlider.value = newValue;
    }
    
    public void UpdateShield(float newValue)
    {
        shieldSlider.value = newValue;
    }
}
