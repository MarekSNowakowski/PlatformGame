using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Continue()
    {
        //TBD
    }

    public void NewGame()
    {
        SceneManager.LoadScene("StarterMap");
    }

    public void Options()
    {
        //TBD
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
