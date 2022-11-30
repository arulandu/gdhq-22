using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//
//
//WHEN YOU USE THIS DISABLE CINEMACHINE VCAM
//
//


public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 0.2f;
    Vector2 _dirInput;
    float dX;
    float dY;
    float dZ;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationY = 0F;
    void Update ()
    {

        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        


        _dirInput.y = Input.GetAxis("Vertical");
        _dirInput.x = Input.GetAxis("Horizontal");

        if (Input.GetKey("space"))
            dY = cameraSpeed;
        else if (Input.GetKey("left shift"))
            dY = -1 * cameraSpeed;
        else 
            dY = 0;
        dZ = cameraSpeed * _dirInput.y;
        dX = cameraSpeed * _dirInput.x;

        

        transform.position = transform.position + (dZ * transform.forward) + dY * transform.up + dX * transform.right;
    }
}
