// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TutorialManager : MonoBehaviour
{
    private SceneLoader _loader;

    private void Awake()
    {
        _loader = FindObjectOfType<SceneLoader>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
            _loader.LoadScene("MainMenu");
    }
}
