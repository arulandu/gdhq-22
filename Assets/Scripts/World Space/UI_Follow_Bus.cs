using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UI_Follow_Bus : MonoBehaviour
{
    public GameObject busGameObject;

    Quaternion busRotation;
    Vector3 busPosition;

    
    Vector3 busUpNormal; //normal vector that represents a vector directly upwards from the bus
    Vector3 busRightNormal; //normal vector that represents a vector directly rightwards from the bus
    Vector3 busForwardNormal; //normal vector that represents a vector directly forwards from the bus

    public float vertUIDisplacement; //vertical displacement of the UI from the bus (Should be set in editor)
    public float horUIDisplacement; //horizontal displacement (x axis) of the UI from the bus (Should be set in editor)
    public float horzUIDisplacement; //horizontal displacment (z axis) of the UI from the bus (Should be set in editor)

    void FixedUpdate(){

        //quickly grab this cause it'll be useful later
        busPosition = busGameObject.transform.position;

        //find vectors that points 90 degrees to the right of the bus and 90 degrees upwards
        busForwardNormal = busGameObject.transform.forward;
        busUpNormal = busGameObject.transform.up;
        busRightNormal = busGameObject.transform.right;

        //after you find that vector, use vector addition to place the UI element based on upUIPos and rightUIPos
        transform.position = busPosition + (busUpNormal * vertUIDisplacement) + (busRightNormal * horUIDisplacement) + (busForwardNormal * horzUIDisplacement);

        //next rectify the rotation of the ui to the rotation of the bus (which should roughly be the rotation of the camera)
        transform.rotation = busGameObject.transform.rotation;
    }
    
}
