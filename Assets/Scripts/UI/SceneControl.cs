using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{

    public void startLevel(int levelNumber) {

        GameObject.FindObjectOfType<LevelSelector>().currentLevel = levelNumber;
        SceneManager.LoadScene("CarPlayground");
    }

    public void levelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
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
