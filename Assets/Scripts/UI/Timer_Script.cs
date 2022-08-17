// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer_Script : MonoBehaviour
{

    public uint totalSeconds; //set this in the editor
    public uint totalMinutes; //set this in the editor
    private static uint framesLeft; 

    public static TextMeshPro timerText;

    //stores all variables and methods that can turn the frame count into a timer string
    struct timeFormat{
        
        static uint seconds;
        static uint minutes;

        public static void setTime(uint framesLeft_){
        
            minutes = framesLeft_ / 3600;
            framesLeft_ -= minutes * 3600;

            seconds = framesLeft_ / 60;
        }

        public static string toString(){
            return minutes + ":" + seconds;
        }

    }

    //Updates the UI
    //Called whenever you want to write the frame number to the timerText
    static void UIUpdate(){

        timeFormat.setTime(framesLeft);
        timerText.text = timeFormat.toString();
    }

    //Do whatever method you want whenever the player game overs
    static void gameOver(){} //TODO

    //Initialize everything
    void Awake(){

        timerText = gameObject.GetComponent<TextMeshPro>();

        framesLeft = (totalMinutes * 3600) + (totalSeconds * 60);
        
        timeFormat.setTime(framesLeft);
        timerText.text = timeFormat.toString();
    }

    void FixedUpdate(){

        if (framesLeft == 0)
            gameOver();

        framesLeft--;

        UIUpdate();
    }
}
