// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer_Script : MonoBehaviour
{

    public static Timer_Script timer_Script;
    public gameOver gameOverScript;

    public static uint totalSeconds; //set this in the editor
    public static uint totalMinutes; //set this in the editor
    public static uint framesLeft; 

    public static uint dangerModeSeconds = 30;
    public static uint dangerModeMinutes = 0;

    public bool isFinalCountdown = false;

    public static TextMeshProUGUI timerText;
    public static TextMeshProUGUI finalCountdownText;

    public Color dangerModeColor;


    //stores all variables and methods that can turn the frame count into a timer string
    struct timeFormat{
        
        public static uint seconds;
        public static uint minutes;

        public static void setTime(uint framesLeft_){
        
            minutes = framesLeft_ / 3600;
            framesLeft_ -= minutes * 3600;

            seconds = framesLeft_ / 60;
        }

        public static string toString(){
            if (seconds < 10)
                return minutes + ":0" + seconds;
            else
                return minutes + ":" + seconds;
        }

    }

    //Updates the UI
    //Called whenever you want to write the frame number to the timerText
    void UIUpdate(){

        
        timeFormat.setTime(framesLeft);

        if (timeFormat.minutes <= dangerModeMinutes && timeFormat.seconds <= dangerModeSeconds)
            enableDangerMode();

        if (!isFinalCountdown)
            timerText.text = "Time Left: " + timeFormat.toString();
        else  
            finalCountdownText.text = timeFormat.toString();
        
    }

    //makes the text become red and might make some sound thing play whenever the timer is below a certain threshold
    static void enableDangerMode(){

        timerText.color = timer_Script.dangerModeColor;
    }

    void enableFinalCountDown(){

        timerText.enabled = false;
        isFinalCountdown = true;
    }


    //Do whatever method you want whenever the player game overs
    static void gameOver(){
        //pause the game

        //Display a "Stop Sign" when the game is over

        //Display score

    } //TODO

    //Initialize everything
    void Awake(){

        finalCountdownText = transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>();
        timer_Script = gameObject.GetComponent<Timer_Script>();
        timerText = gameObject.GetComponent<TextMeshProUGUI>();

    }

    //called from level selector
    public static void setTime(uint totalMinutes_, uint totalSeconds_) {

        Debug.Log("Setting Time");
        totalMinutes = totalMinutes_;
        totalSeconds = totalSeconds_;

        framesLeft = (totalMinutes * 3600) + (totalSeconds * 60);    
        timeFormat.setTime(framesLeft);

        timerText.text = timeFormat.toString();

        Debug.Log("Setting Time");
    }
    

    void FixedUpdate(){

        if (framesLeft == 0) {

            finalCountdownText.text = "";
            gameOverScript.startGameOver();
            gameObject.SetActive(false); //make sure it stops calling startGameOver() after it is already called
        }
        else
            framesLeft--;

        if (framesLeft < 240)
            enableFinalCountDown();

        UIUpdate();
    }
}
