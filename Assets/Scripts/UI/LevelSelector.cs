// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{  
    public Texture2D[] levels; //set in the editor
    public int currentLevel; //change this in order to change the level that you are currently on
    public string mainSceneName = "carPlayground";

    void OnSceneLoaded(Scene scene){

        if (scene.name == mainSceneName){
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CityGenerator"))
                obj.GetComponent<CityGenerator>().pattern = levels[currentLevel - 1];
        }
    }

    void Awake(){
        DontDestroyOnLoad(this);
    }
}