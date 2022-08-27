// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//
//
//MOST OF THIS STUFF DOESN'T WORK CURRENTLY
//
//
//



public class gameOver : MonoBehaviour
{
    public GameObject stopSignObject;
    
    public Score_Script scoreScript;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalNumChildrenDroppedOffText;

    public Vector3 stopSignInitialPos;
    public Vector3 stopSignFinalPos;
    [Range(0, 1)]
    public float stopSignPos; //lerp fraction between two points 
    public float stopSignSpeed; //how the previous changes over time

    bool isDisplayingStopSign = false;



    public void startGameOver() {

        Debug.Log("Starting Game Over");
        lockControls();
        displayStopSign();
        destroyCity();
        displayScore();
    }   


    void lockControls() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EventSystem")) 
            obj.SetActive(false);
    }


    void displayStopSign() {
        isDisplayingStopSign = true;
    }

    void destroyCity() { //for lag reasons

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CityGenerator"))
            Destroy(obj);
    }

    void displayScore() {

        scoreText.text = "Score: " + scoreScript.getScore();
        totalNumChildrenDroppedOffText.text = "Number of Children Dropped Off: " + Bus.totalNumChildrenDroppedOff;
    }

    void exitToMainMenu() {

        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }


    //Helper methods
    void moveStopSign() { //move stop sign to the center of the screen
        
        stopSignPos += stopSignSpeed;
        Vector3.Lerp(stopSignInitialPos, stopSignFinalPos, stopSignPos);
    }

    void Awake() {
        stopSignInitialPos = stopSignObject.transform.position;
    }

    void Update() {

        if (isDisplayingStopSign)
            moveStopSign();
    }
}
