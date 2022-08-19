// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;

//helper class (mostly for organization) that controls the special effects that the score text can go through depending on the situation
public class Score_Effects {

    public TextMeshProUGUI textMeshProUGUI; //same one as in the Score_Script

    public Score_Effects(TextMeshProUGUI textMeshProUGUI_){

        textMeshProUGUI = textMeshProUGUI_;
    }

    public void popScore(){} //makes the score pop out probably would be called when the player drops off children
    public void enterDangerMode(){} //makes the text red and maybe makes it blink in someway when the score is beneath a certain threshold (maybe?)
    public void enlargenScore(int score){} //makes the text slighty bigger depending on how large the score is. The bigger the score, the bigger the score's text to emphasize having a large score

}


public class Score_Script : MonoBehaviour
{
    private uint score;

    public float timeMultiplier; //set in the editor and SHOULD be tweeked
    public float childrenMultiplier; //set in the editor and SHOULD be tweeked
    public float initialScore; //set in the editor and SHOULD be tweeked
    public bool isInvisible = true; //can be tweaked in the editor

    private float scoreAdder; //score that you would need to add to start with the initial score that is desired

    public TextMeshProUGUI scoreText;

    public Score_Effects scoreEffects;

    //this is called in start so that framesLeft is already initialized since Start is called after Awake
    void Start(){

        scoreAdder = initialScore - (float)(Timer_Script.framesLeft * timeMultiplier);
        score = (uint)initialScore;

        scoreEffects = new Score_Effects(scoreText);

        if (isInvisible)
            this.gameObject.GetComponent<TextMeshProUGUI>().color = new Color (0, 0, 0, 0); //make the text transparent 
    }
    

    void FixedUpdate(){

        Debug.Log(/*CarController*/cardrive.totalNumChildrenDroppedOff);
        
        //make sure that the player doesn't have negative points
        if ((scoreAdder + ((float)/*CarController*/cardrive.totalNumChildrenDroppedOff * childrenMultiplier) + ((float)Timer_Script.framesLeft * timeMultiplier)) <= 0)
            score = 0;

        //apply the points formula
        else
            score = (uint) (scoreAdder + ((float)/*CarController*/ cardrive.totalNumChildrenDroppedOff * childrenMultiplier) + ((float)Timer_Script.framesLeft * timeMultiplier));
        
        
        scoreText.text = score.ToString() + "pt";
    }
}