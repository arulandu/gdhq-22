//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Pick_Up_Zone_Collisions : MonoBehaviour
{
    bool alreadyVisited;
    public uint numChildrenAtPickUp; //number of children at this pick up zone
    static GameObject busObject; //initialize this in the editor to whatever collider the bus is using

    void Awake(){

        //just make sure this variable is initalized to false when the game starts
        alreadyVisited = false;
    }


    //when one of the drop zones detects a collision, double check if it is colliding with the car and then call the Drop_Off function
    void OnTriggerEnter(Collider collider){


        if (collider == busObject.GetComponent<BoxCollider>() && alreadyVisited == false){

            alreadyVisited = true;
            BusController.pickUp(numChildrenAtPickUp);
        }
        
    }

    public static void setBus(GameObject busObject_) {
        busObject = busObject_;
    }
}
