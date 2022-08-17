using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
     
public class CarController : MonoBehaviour {
    public List<AxleInfo> axleInfos;
    public float antiRoll = 5000.0f;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    private Rigidbody _rb;

    public static uint numChildren;

    public static void pickUp(uint numChildrenPickingUp){

        numChildren += numChildrenPickingUp;
    }

    public static void dropOff(){

        numChildren = 0;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
     
    public void FixedUpdate()
    {
        Debug.Log(numChildren);

        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
     
        foreach (AxleInfo axleInfo in axleInfos) {
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
    }

}