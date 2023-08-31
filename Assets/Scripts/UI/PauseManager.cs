using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private bool isPaused;
    public GameObject pausePanel;

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
                pausePanel.SetActive(true);
                //Time.timeScale = 0f;
            }
            else
            {
                pausePanel.SetActive(false);
                //Time.timeScale = 1f;
            }
        }
    }

    public void Resume()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void QuitToMenu()
    {
        pausePanel.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
