using UnityEngine;

public class MainMenuSoundsManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private ManagedAudioSource buttonClickSound;
    [SerializeField] private ManagedAudioSource playClicked;
    [SerializeField] private ManagedAudioSource cancelClicked;

    public void PlayButtonClickedSound()
    {
        buttonClickSound.AudioSource.Play();
    }
    
    public void PlayPlayClickedSound()
    {
        playClicked.AudioSource.Play();
    }

    public void PlayCancelClickedSound()
    {
        cancelClicked.AudioSource.Play();
    }
}
