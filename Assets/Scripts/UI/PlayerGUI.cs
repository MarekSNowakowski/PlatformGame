using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private Slider shieldSlider;

    public void InitializeName(string name)
    {
        playerName.text = name;
    }
    
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
