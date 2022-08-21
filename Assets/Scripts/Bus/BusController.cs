using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BusAxleInfo {

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
     


public class BusController : MonoBehaviour {

    public List<BusAxleInfo> axleInfos;
    public float antiRoll = 5000.0f;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Transform cOM;
    private Vector2 _dirInput;

    private Rigidbody _rb;

    public static uint numChildren;
    public static uint totalNumChildrenDroppedOff;
    public static bool isFlippedOver;


    public static void pickUp(uint numChildrenPickingUp){

        numChildren += numChildrenPickingUp;
    }

    public static void dropOff(){
        
        Debug.Log("Dropping Off: " + numChildren);
        totalNumChildrenDroppedOff += numChildren;
        numChildren = 0;
    }
    




    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = transform.InverseTransformPoint(cOM.position);

        Drop_Zone_Collisions.setBus(gameObject);
        Pick_Up_Zone_Collisions.setBus(gameObject);
    }

    private void Update()
    {
        _dirInput.y = Input.GetAxis("Vertical");
        _dirInput.x = Input.GetAxis("Horizontal");
    }

    private void OnDrawGizmos()
    {
        _rb = GetComponent<Rigidbody>();
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation*_rb.centerOfMass, 1f);
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
    
    public static void Explode() {

    }


    public void FixedUpdate()
    {
        float motor = maxMotorTorque * _dirInput.y;
        float steering = maxSteeringAngle * _dirInput.x;
     
        foreach (BusAxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            
            // anti-roll bars? https://forum.unity.com/threads/how-to-make-a-physically-real-stable-car-with-wheelcolliders.50643/
            WheelHit hit;
            var groundedL = axleInfo.leftWheel.GetGroundHit(out hit);
            var groundedR = axleInfo.rightWheel.GetGroundHit(out hit);
            var travelL = groundedL ? (axleInfo.leftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheel.radius) / axleInfo.leftWheel.suspensionDistance : 1.0f;
            var travelR = groundedL ? (axleInfo.rightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.rightWheel.radius) / axleInfo.rightWheel.suspensionDistance : 1.0f;

            var antiRollForce = (travelL-travelR) *antiRoll;
            
            if(groundedL) _rb.AddForceAtPosition(axleInfo.leftWheel.transform.up*-antiRollForce, axleInfo.leftWheel.transform.position);
            if(groundedR) _rb.AddForceAtPosition(axleInfo.rightWheel.transform.up*-antiRollForce, axleInfo.rightWheel.transform.position);
            
            // update wheel graphics
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }


        //just a small check to see if the vehicle is flipped over
        if ((Math.Abs(transform.eulerAngles.x) >= 90 && Math.Abs(transform.eulerAngles.x) <= 270) ||
            (Math.Abs(transform.eulerAngles.z) >= 90 && Math.Abs(transform.eulerAngles.z) <= 270)) {

                isFlippedOver = true;
                Explode();
        }
    }
}
