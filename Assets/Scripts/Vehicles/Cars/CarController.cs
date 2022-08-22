using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
     

public class CarController : MonoBehaviour {
    private Vector2 _dirInput;

    private Rigidbody _rb;
    private AutomobileController _automobile;
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _automobile = GetComponent<AutomobileController>();
    }

    public void FixedUpdate()
    {
        //steering AI
        _dirInput = Autopilot(transform.position.x, transform.position.y);
        _automobile.Drive(_dirInput);
    }
    
    public Vector2 Autopilot(float xPos, float yPos) {
        return new Vector2 (0, 0);
    }

}