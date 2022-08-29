using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bus : MonoBehaviour
{
    public GameObject collectFx;
    //these will be static for right now 
    //if we would ever consider multiplayer (which we won't for the purposes of this game jam) we would have to rewrite a lot of the child drop off and pick up code

    public static uint numChildren;
    public static uint totalNumChildrenDroppedOff;


    public void pickUp(uint numChildrenPickingUp)
    {
        numChildren += numChildrenPickingUp;
        var obj = Instantiate(collectFx, transform.position, Quaternion.identity);
        Destroy(obj, 5);
    }

    public static void dropOff()
    {
        Debug.Log("Dropping Off: " + numChildren);
        totalNumChildrenDroppedOff += numChildren;
        numChildren = 0;

        if (totalNumChildrenDroppedOff == GameObject.FindObjectOfType<CityGenerator>().totalNumChildren)
            GameObject.FindObjectOfType<gameOver>().startGameOver();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        numChildren = 0;
        totalNumChildrenDroppedOff = 0;
        Debug.Log("OnSceneLoaded");
    }
    
    void Awake(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


}
