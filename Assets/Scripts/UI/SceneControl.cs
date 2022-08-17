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
}
