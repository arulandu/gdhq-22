using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class gameOver : MonoBehaviour
{
    public GameObject stopSignObject;
    public GameObject miniMapObject;
    public GameObject timerObject;
    
    public BusController busController;
    public Score_Script scoreScript;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalNumChildrenDroppedOffText;
    public TextMeshProUGUI promptText; //prompts the user to press any button to return to the main menu
    public TextMeshProUGUI clearText;

    public Vector3 stopSignInitialPos;
    

    public float stopSignPos; //lerp fraction between two points 
    public float stopSignSpeed; //how the previous changes over time

    bool isMovingStopSign = false;
    bool isWaitingForStopSign = false;
    bool isWaitingForButtonPress = false;
    Vector3 stopSignFinalPos = new Vector3(Screen.width / 2, Screen.height / 2);

    public float waitAfterStopSign = 2f;

    public void startGameOver() {

        Debug.Log("Starting Game Over");
        lockControls();
        disableOverlays();
        displayStopSign();
        //displayScore();
    }   


    void lockControls() {      
        busController.takeInput = false;
    }

    void disableOverlays() {
        miniMapObject.SetActive(false);
        timerObject.SetActive(false);

    }

    void displayStopSign() {
        isMovingStopSign = true;
    }

    void moveStopSign() { //move stop sign to the center of the screen
        
        stopSignPos += stopSignSpeed;
        stopSignObject.transform.position = Vector3.Lerp(stopSignInitialPos, stopSignFinalPos, stopSignPos);
        // Debug.Log(stopSignPos);
    }

    void displayScore() {

        scoreText.text = "Time: " + Timer_Script.timeFormat.timeElapsed();
        totalNumChildrenDroppedOffText.text = "Number of Children Dropped Off: " + Bus.totalNumChildrenDroppedOff;

        // if (GameObject.FindObjectOfType<CityGenerator>().totalNumChildren == Bus.totalNumChildrenDroppedOff)
        //     clearText.text = "STAGE CLEARED!";

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

        else if (isWaitingForStopSign) {} //called third just to make sure update doesnt spam coroutine

        else if (stopSignPos >= 1) //if the stop sign is at its final position  //called second
            StartCoroutine(inStopSignWait());

        else if (isMovingStopSign)  //called first
            moveStopSign();
    }



    IEnumerator inStopSignWait() {

        isWaitingForStopSign = true;
        displayScore();
        
        yield return new WaitForSeconds(waitAfterStopSign);
        
        isWaitingForButtonPress = true;
        displayPrompt();
    }
}
