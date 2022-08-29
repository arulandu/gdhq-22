// using System.Collections;
// using System.Collections.Generic;

using System;
using UnityEngine;
using TMPro;

public class Timer_Script : MonoBehaviour
{

    public static Timer_Script timer_Script;
    public gameOver gameOverScript;
    public uint totalSeconds; //set this in the editor
    public uint totalMinutes; //set this in the editor
    public uint endHours, endMinuts, endSeconds;
    [SerializeField]
    public static uint framesLeft, framesToGo; 
    [SerializeField]
    public static uint totalFrames;

    public static uint dangerModeSeconds = 30;
    public static uint dangerModeMinutes = 0;

    public bool isFinalCountdown = false;

    public static TextMeshProUGUI timerText;
    public static TextMeshProUGUI finalCountdownText;

    public Color dangerModeColor;
    public TimeFormat _currentTime, _startTime;

    //stores all variables and methods that can turn the frame count into a timer string
    public class TimeFormat {
        
        public uint seconds;
        public uint minutes;
        public uint hours;
        public uint totalFrames;

        public TimeFormat(uint seconds, uint minutes, uint hours)
        {
            this.seconds = seconds;
            this.minutes = minutes;
            this.hours = hours;
            totalFrames = seconds + minutes * 60 + hours * 3600;
        }
        
        public void setTime(uint framesLeft_){
            
            totalFrames = framesLeft_;

            minutes = framesLeft_ / 3600;
            framesLeft_ -= minutes * 3600;

            seconds = framesLeft_ / 60;
        }

        public void updateTime (uint framesLeft_) {
            minutes = framesLeft_ / 3600;
            framesLeft_ -= minutes * 3600;

            seconds = framesLeft_ / 60;
        }

        public string toString(){
            if (seconds < 10)
                return hours + ":" + minutes + ":0" + seconds;
            else
                return hours + ":" + minutes + ":" + seconds;
        }

        public string timeElapsed() {
            updateTime(totalFrames - (seconds * 60 + minutes * 3600));
            return toString();
        }

    }

    //Updates the UI
    //Called whenever you want to write the frame number to the timerText
    void UIUpdate(){
        _currentTime.updateTime(framesToGo);

        if (_currentTime.minutes <= dangerModeMinutes && _currentTime.seconds <= dangerModeSeconds)
            enableDangerMode();

        if (!isFinalCountdown)
            timerText.text = "Time: " + _currentTime.toString() + " AM / " + _startTime.toString() + " AM";
        else  
            finalCountdownText.text = _currentTime.toString();
        
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

    private void Start()
    {
        Debug.Log("Setting Time");

        framesLeft = (totalMinutes * 3600) + (totalSeconds * 60);
        framesToGo = ((endMinuts-totalMinutes) * 3600) + ((endSeconds-totalSeconds) * 60);
        _currentTime = new TimeFormat(endSeconds-totalSeconds, endMinuts-totalMinutes, endHours);
        _startTime = new TimeFormat(endSeconds, endMinuts, endHours);

        timerText.text = _currentTime.toString();

        Debug.Log("Setting Time");
    }

    void FixedUpdate(){

        if (framesLeft == 0) {

            finalCountdownText.text = "";
            gameOverScript.startGameOver();
            gameObject.SetActive(false); //make sure it stops calling startGameOver() after it is already called
        }
        else{
            framesLeft--;
            framesToGo++;
        }

        if (framesLeft < 240)
            enableFinalCountDown();

        UIUpdate();
    }
}
