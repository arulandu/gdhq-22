// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer_Script : MonoBehaviour
{

    public static Timer_Script timer_Script;

    public uint totalSeconds; //set this in the editor
    public uint totalMinutes; //set this in the editor
    public static uint framesLeft; 

    public static uint dangerModeSeconds = 30;
    public static uint dangerModeMinutes = 0;

    public static TextMeshProUGUI timerText;

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
    static void UIUpdate(){

        timeFormat.setTime(framesLeft);

        if (timeFormat.minutes <= dangerModeMinutes && timeFormat.seconds <= dangerModeSeconds)
            enableDangerMode();

        timerText.text = timeFormat.toString();
    }

    //makes the text become red and might make some sound thing play whenever the timer is below a certain threshold
    static void enableDangerMode(){

        timerText.color = timer_Script.dangerModeColor;
    }

    //Do whatever method you want whenever the player game overs
    static void gameOver(){} //TODO

    //Initialize everything
    void Awake(){

        timer_Script = gameObject.GetComponent<Timer_Script>();

        timerText = gameObject.GetComponent<TextMeshProUGUI>();

        framesLeft = (totalMinutes * 3600) + (totalSeconds * 60);    
        timeFormat.setTime(framesLeft);
        timerText.text = timeFormat.toString();
    }

    void FixedUpdate(){

        if (framesLeft == 0)
            gameOver();
        else
            framesLeft--;

        UIUpdate();
    }
}
