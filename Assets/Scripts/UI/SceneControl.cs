using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene("CarPlayground");
    }

    public void optionsMenu()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
