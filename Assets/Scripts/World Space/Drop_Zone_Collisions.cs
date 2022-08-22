//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Drop_Zone_Collisions : MonoBehaviour
{
    static GameObject busObject; //initialize this in the editor to the bus object

    //when one of the drop zones detects a collision, double check if it is colliding with the car and then call the Drop_Off function
    void OnTriggerEnter(Collider collider){

        if (collider == busObject.GetComponent<BoxCollider>())
            Bus.dropOff();
               
    }

    public static void setBus(GameObject busObject_) {
        busObject = busObject_;
    }
}
