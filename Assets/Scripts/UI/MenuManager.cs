using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private SceneLoader _loader;

    private void Awake()
    {
        _loader = FindObjectOfType<SceneLoader>();
    }

    public void ToMenu()
    {
        _loader.LoadScene("MainMenu");
    }
    
    public void ToOptions()
    {
        _loader.LoadScene("OptionsMenu");
    }

    public void ToGame()
    {
        _loader.LoadScene("Game");
    }

    public void ToTutorial()
    {
        _loader.LoadScene("Tutorial");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
