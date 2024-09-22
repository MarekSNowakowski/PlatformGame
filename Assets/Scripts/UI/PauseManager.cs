using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool isPaused;
    public GameObject pausePanel;

    [SerializeField]
    private ManagedAudioSource onPauseSound;
    [SerializeField]
    private ManagedAudioSource onUnpauseSound;
    [SerializeField]
    private ManagedAudioSource onQuitSound;
    
    void Start()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetButtonDown("pause"))
        {
            isPaused = !isPaused;
            if(isPaused)
            {
                onPauseSound.AudioSource.Play();
                pausePanel.SetActive(true);
                //Time.timeScale = 0f;
            }
            else
            {
                onUnpauseSound.AudioSource.Play();
                pausePanel.SetActive(false);
                //Time.timeScale = 1f;
            }
        }
    }

    public void Resume()
    {
        onUnpauseSound.AudioSource.Play();
        isPaused = !isPaused;
        pausePanel.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void QuitToMenu()
    {
        onQuitSound.AudioSource.Play();
        pausePanel.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
