using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        Time.timeScale = 1;
    }

    public void OptionsClicked()
    {
        // TBD    
    }
    
    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
