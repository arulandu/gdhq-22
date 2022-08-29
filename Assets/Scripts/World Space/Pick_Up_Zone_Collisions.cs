using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Pick_Up_Zone_Collisions : MonoBehaviour
{
    public ParticleSystem collectFX;
    public Transform collectFXPos;
    bool alreadyVisited;
    public uint numChildrenAtPickUp; //number of children at this pick up zone
    static GameObject busObject; //initialize this when it is instantiated
    GameObject houseObject; //initialize this when it is instantiated
    
    public AudioSource pickUpAudio;


    void Awake(){

        //just make sure this variable is initalized to false when the game starts
        alreadyVisited = false;
    }


    //when one of the drop zones detects a collision, double check if it is colliding with the car and then call the Drop_Off function
    void OnTriggerEnter(Collider collider){
        if (collider == busObject.GetComponent<BoxCollider>() && alreadyVisited == false){

            alreadyVisited = true;
            Bus.pickUp(numChildrenAtPickUp);
            houseObject.GetComponent<houseScript>().removeChild();
            
            collectFXPos.position = transform.position;
            if (!collectFX.isPlaying)
            {
                if (!collectFX.enableEmission)
                {
                    collectFX.enableEmission = true;
                }

                collectFX.Play();
                pickUpAudio.Play();
            }
        }
    }

    public static void setBus(GameObject busObject_) {
        busObject = busObject_;
    }

    public void setHouse (GameObject houseObject_) {
        houseObject = houseObject_;
    }

}
