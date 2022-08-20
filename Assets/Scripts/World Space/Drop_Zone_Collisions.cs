//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Drop_Zone_Collisions : MonoBehaviour
{
    public Collider busCollider; //initialize this in the editor to whatever collider the bus is using

    //when one of the drop zones detects a collision, double check if it is colliding with the car and then call the Drop_Off function
    void OnTriggerEnter(Collider collider){

        if (collider == busCollider)
            cardrive.dropOff();
               
    }
}
