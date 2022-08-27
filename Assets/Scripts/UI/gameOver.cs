using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class gameOver : MonoBehaviour
{
    public GameObject stopSignObject;
    
    public Score_Script scoreScript;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalNumChildrenDroppedOffText;
    public TextMeshProUGUI promptText; //prompts the user to press any button to return to the main menu

    public Vector3 stopSignInitialPos;
    public Vector3 stopSignFinalPos;
    [Range(0, 1)]
    public float stopSignPos; //lerp fraction between two points 
    public float stopSignSpeed; //how the previous changes over time

    bool isMovingStopSign = false;
    bool isWaitingForStopSign = false;
    bool isWaitingForButtonPress = false;

    public float waitAfterStopSign = 2f;

    public void startGameOver() {

        Debug.Log("Starting Game Over");
        lockControls();
        displayStopSign();
        //displayScore();
    }   


    void lockControls() {      
        BusController.takeInput = false;
    }

    void displayStopSign() {
        isMovingStopSign = true;
    }

    void moveStopSign() { //move stop sign to the center of the screen
        
        stopSignPos += stopSignSpeed;
        stopSignObject.transform.position = Vector3.Lerp(stopSignInitialPos, stopSignFinalPos, stopSignPos);
        Debug.Log(stopSignPos);
    }

    void displayScore() {

        scoreText.text = "Score: " + scoreScript.getScore();
        totalNumChildrenDroppedOffText.text = "Number of Children Dropped Off: " + Bus.totalNumChildrenDroppedOff;
    }

    void displayPrompt() {
        promptText.gameObject.SetActive(true);
    }

    void exitToMainMenu() {

        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }



    void destroyCity() { //for lag reasons

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("CityGenerator"))
            Destroy(obj);
    }




    void Awake() {
        stopSignInitialPos = stopSignObject.transform.position;
    }

    void Update() { //this is terribly coded but whatever

        if (isWaitingForButtonPress && Input.anyKey) //called last
            exitToMainMenu();
        
        else if (isWaitingForButtonPress) //called fourth
            displayPrompt();
    
        else if (isWaitingForStopSign) //called third
            displayScore();

        else if (stopSignPos >= 1) //if the stop sign is at its final position  //called second
            StartCoroutine(inStopSignWait());

        else if (isMovingStopSign)  //called first
            moveStopSign();
    }



    IEnumerator inStopSignWait() {

        isWaitingForStopSign = true;

        yield return new WaitForSeconds(waitAfterStopSign);
        
        isWaitingForButtonPress = true;
    }
}
