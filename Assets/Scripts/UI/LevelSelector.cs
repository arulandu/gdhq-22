// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level {

    public Texture2D texture;
    public uint size;
    public uint totalSeconds;
    public uint totalMinutes;
    public Vector3 busSpawnPos;
}

public class LevelSelector : MonoBehaviour
{  
    public Level[] levels; //set in the editor
    int currentLevel = 2; //change this in order to change the level that you are currently on
    public string mainSceneName = "CarPlayground";

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode){
 
        if (scene.name == mainSceneName){
            
            Timer_Script.setTime(levels[currentLevel - 1].totalMinutes, levels[currentLevel - 1].totalSeconds);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Bus"))
                obj.transform.position = levels[currentLevel - 1].busSpawnPos;

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Border"))
                obj.GetComponent<borderScript>().setSize(levels[currentLevel - 1].size);

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CityGenerator"))
                obj.GetComponent<CityGenerator>().pattern = levels[currentLevel - 1].texture;
            
        }
    }

    void Start(){
        DontDestroyOnLoad(this);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
}