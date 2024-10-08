using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject soundOffIcon;
    [SerializeField] private GameObject soundOnIcon;

    private bool beforeInitialization = true;

    private void Start()
    {
        volumeSlider.value = AudioProperties.RealPreferredVolume;
        if (AudioProperties.Muted)
            ToggleMuteVisually();

        beforeInitialization = false;
    }

    private void ToggleMuteVisually()
    {
        soundOffIcon.SetActive(AudioProperties.Muted);
        soundOnIcon.SetActive(!AudioProperties.Muted);
    }

    public void ToggleMute()
    {
        AudioProperties.ToggleMute();

        ToggleMuteVisually();
    }

    public void OnSliderChanged()
    {
        if (beforeInitialization)
            return;

        AudioProperties.RealPreferredVolume = volumeSlider.value;

        if ((volumeSlider.value == 0f && !AudioProperties.Muted) || (volumeSlider.value > 0f && AudioProperties.Muted))
            ToggleMute();
    }
}
