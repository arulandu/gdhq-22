using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SceneLoader _loader;
    // Start is called before the first frame update
    void Start()
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
}
