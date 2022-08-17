//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Pick_Up_Zone_Collisions : MonoBehaviour
{
    bool alreadyVisited = false;
    public uint numChildrenAtPickUp; //number of children at this pick up zone
    public Collider busCollider; //initialize this in the editor to whatever collider the bus is using

    //when one of the drop zones detects a collision, double check if it is colliding with the car and then call the Drop_Off function
    void OnTriggerEnter(Collider collider){

        if (collider == busCollider && alreadyVisited == false){

            alreadyVisited = true;
            CarController.pickUp(numChildrenAtPickUp);
        }
        
    }
}
