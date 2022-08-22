using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BusController : MonoBehaviour {

    public static void pickUp(uint numChildrenPickingUp){

        numChildren += numChildrenPickingUp;
    }

    public static void dropOff(){
        
        Debug.Log("Dropping Off: " + numChildren);
        totalNumChildrenDroppedOff += numChildren;
        numChildren = 0;
    }


    public List<AxleInfo> axleInfos;
    public float antiRoll = 5000.0f;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Transform cOM;
    private Vector2 _dirInput;

    private Rigidbody _rb;

    //these will be static for right now 
    //if we would ever consider multiplayer (which we won't for the purposes of this game jam) we would have to rewrite a lot of the child drop off and pick up code
    public static uint numChildren;
    public static uint totalNumChildrenDroppedOff;
    public static bool isFlippedOver;


    //drifting
    Vector3 direction;
    Vector3 prevPos;
    Vector3 velocity;
    float currentDrift;
    float totalDrift;
    public float driftResetThreshold; //if the drift is below this threshold then the driftTotal resets
    float prevVelocityMagnitude;
    //drifting stats
    public float miniBoostThreshold; //How high the drift -float- has to be before a miniBoost is initiated
    public float miniBoostSpeed; //Speed you get from a mini boost
    public float midBoostThreshold; //How high the drift -float- has to be before a midBoost is initiated
    public float midBoostSpeed; //Speed you get from a mid boost
    public float turboBoostThreshold; //How high the drift -float- has to be before a turboBoost is initiated
    public float turboBoostSpeed; //Speed you get from a turbo boost


    [System.Serializable]
    public struct AxleInfo {

        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
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



    public void calculateDrift() {

        direction = transform.forward;
        velocity = (transform.position - prevPos)/ Time.fixedDeltaTime;
        prevPos = transform.position;

        currentDrift = Vector3.Cross(direction, velocity).magnitude;
        totalDrift += currentDrift;

        if (currentDrift <= driftResetThreshold) //reset the total drift if it is too low
            totalDrift = 0;
    }


    //Boosts
    //Add a force to the car when boosting (keeping the addForceAtPosition stuff consistant with the fixedUpdate() stuff)
    public void miniBoost() {
        
        Debug.Log(totalDrift);
        _rb.velocity = transform.forward * _rb.velocity.magnitude;
        //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
        foreach (AxleInfo axleInfo in axleInfos) {

            _rb.AddForceAtPosition(transform.forward * (miniBoostSpeed), axleInfo.leftWheel.transform.position);
            _rb.AddForceAtPosition(transform.forward * (miniBoostSpeed), axleInfo.rightWheel.transform.position);
        }

        Debug.Log("isMiniBoosting");
        totalDrift = 0;
    }

    public void midBoost() {
        
        Debug.Log(totalDrift);
        _rb.velocity = transform.forward * _rb.velocity.magnitude;
        //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
        foreach (AxleInfo axleInfo in axleInfos) {
            _rb.AddForceAtPosition(transform.forward * (midBoostSpeed), axleInfo.leftWheel.transform.position);
            _rb.AddForceAtPosition(transform.forward * (midBoostSpeed), axleInfo.rightWheel.transform.position);
        }

        Debug.Log("isMidBoosting");
        totalDrift = 0;
    }

    public void turboBoost() {

        Debug.Log(totalDrift);
        _rb.velocity = transform.forward * _rb.velocity.magnitude;
        //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
        foreach (AxleInfo axleInfo in axleInfos) {
            _rb.AddForceAtPosition(transform.forward * (turboBoostSpeed), axleInfo.leftWheel.transform.position);
            _rb.AddForceAtPosition(transform.forward * (turboBoostSpeed), axleInfo.rightWheel.transform.position);
        }

        Debug.Log("isTurboBoosting");
        totalDrift = 0;
    }


    public void FixedUpdate()
    {
        float motor = maxMotorTorque * _dirInput.y;
        float steering = maxSteeringAngle * _dirInput.x;
     
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


        //just a small check to see if the vehicle is flipped over
        if ((Math.Abs(transform.eulerAngles.x) >= 90 && Math.Abs(transform.eulerAngles.x) <= 270) ||
            (Math.Abs(transform.eulerAngles.z) >= 90 && Math.Abs(transform.eulerAngles.z) <= 270)) {

                isFlippedOver = true;
                Explode();
        }


        //Drifting stuff
        this.calculateDrift();

        if (_dirInput.x == 0 && totalDrift >= turboBoostThreshold)
            turboBoost();

        else if (_dirInput.x == 0 && totalDrift >= midBoostThreshold)
            midBoost();

        else if (_dirInput.x == 0 && totalDrift >= miniBoostThreshold)
            miniBoost();


        //Debug.Log(drift);
    }
}
