using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Num_Children : MonoBehaviour
{
    
    public TextMeshProUGUI numChildrenText;

    //just update the UI each frame not much to say really
    void Update(){

        numChildrenText.text = Bus.numChildren.ToString();
    }
}
