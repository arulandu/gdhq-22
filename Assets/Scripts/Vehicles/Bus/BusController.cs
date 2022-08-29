using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BusController : MonoBehaviour
{
    public AudioSource engineLoop, engineStart, tireSkid;

    public bool isFlippedOver = false;
    [SerializeField]
    private bool _engineOn = false;
    private bool _canHardBreak = true;
    public float hardBreakThreshold = 0.1f;
    private Vector2 _dirInput;
    public bool takeInput = true;

    private Rigidbody _rb;
    private AutomobileController _automobile;

    //boostparticles
    public ParticleSystem boostFX;
    public Transform boostFXPos;
    //drifting
    Vector3 direction;
    Vector3 prevPos;
    Vector3 velocity;
    [SerializeField]
    float currentDrift;
    float totalDrift;
    public float driftResetThreshold; //if the drift is below this threshold then the driftTotal resets

    float prevVelocityMagnitude;

    //drifting stats
    //public float miniBoostThreshold; //How high the drift -float- has to be before a miniBoost is initiated
    //public float miniBoostSpeed; //Speed you get from a mini boost
    public float midBoostThreshold; //How high the drift -float- has to be before a midBoost is initiated
    public float midBoostSpeed; //Speed you get from a mid boost
    public float turboBoostThreshold; //How high the drift -float- has to be before a turboBoost is initiated
    public float turboBoostSpeed; //Speed you get from a turbo boost



    private void Start()
    {
        isFlippedOver = false;
        _rb = GetComponent<Rigidbody>();
        _automobile = GetComponent<AutomobileController>();

        Drop_Zone_Collisions.setBus(gameObject);
        Pick_Up_Zone_Collisions.setBus(gameObject);
    }

    private void Update()
    {
        if (takeInput) {
            _dirInput.y = Input.GetAxis("Vertical");
            _dirInput.x = Input.GetAxis("Horizontal");
        }
        else {
            _dirInput = new Vector2(0,0);
            _rb.velocity = new Vector3(0, 0, 0);
        }

        if (Mathf.Abs(_dirInput.y) > 0.1f && !_engineOn)
        {
            StartCoroutine(StartEngine());
        } else if (Math.Abs(_dirInput.y) < 0.01f && _engineOn)
        {
            StopEngine();
        }
        
        if (_canHardBreak && (Vector3.Cross(direction, velocity.normalized).magnitude > hardBreakThreshold && _rb.velocity.magnitude > 0.2f))
        {
            StartCoroutine(HardBreak());
        }
        
        _automobile.UpdateVisuals();
    }

    IEnumerator StartEngine()
    {
        Debug.Log("start engine");
        _engineOn = true;
        engineStart.Play();
        yield return new WaitForSeconds(0.5f);
        engineLoop.Play();
    }

    void StopEngine()
    {
        Debug.Log("stop engine");
        engineLoop.Stop();
        _engineOn = false;
    }

    IEnumerator HardBreak()
    {
        Debug.Log("hard break");
        _canHardBreak = false;
        tireSkid.Play();
        yield return new WaitForSeconds(1);
        _canHardBreak = true;
    }
    
    
    
    
    
    
    
    public void Explode()
    {
        Debug.Log("Exploding");
    }


    public void calculateDrift()
    {
        direction = transform.forward;
        velocity = (transform.position - prevPos) / Time.fixedDeltaTime;
        prevPos = transform.position;

        currentDrift = _rb.velocity.magnitude > 0.2f ? Vector3.Cross(direction, velocity.normalized).magnitude : 0f;

        if (_dirInput.y >= 0)
            totalDrift += currentDrift;

        if (currentDrift <= driftResetThreshold) //reset the total drift if it is too low
            totalDrift = 0;
    }


    //Boosts
    // //Add a force to the car when boosting (keeping the addForceAtPosition stuff consistant with the fixedUpdate() stuff)
    // public void miniBoost()
    // {
    //     Debug.Log(totalDrift);
    //     _rb.velocity = transform.forward * _rb.velocity.magnitude;
    //     //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
    //     foreach (AxleInfo axleInfo in _automobile.axleInfos)
    //     {
    //         _rb.AddForceAtPosition(transform.forward * (miniBoostSpeed), axleInfo.leftWheel.transform.position);
    //         _rb.AddForceAtPosition(transform.forward * (miniBoostSpeed), axleInfo.rightWheel.transform.position);
    //     }

    //     Debug.Log("isMiniBoosting");
    //     totalDrift = 0;
    // }

    public void midBoost()
    {

        _rb.velocity = transform.forward * _rb.velocity.magnitude;
        //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
        foreach (AxleInfo axleInfo in _automobile.axleInfos)
        {
            _rb.AddForceAtPosition(transform.forward * (midBoostSpeed), axleInfo.leftWheel.transform.position);
            _rb.AddForceAtPosition(transform.forward * (midBoostSpeed), axleInfo.rightWheel.transform.position);
            
        }
        boostFXPos.position = transform.position;
        if (!boostFX.isPlaying)
        {
            if (!boostFX.enableEmission)
            {
                boostFX.enableEmission = true;
            }
            
            boostFX.Play();
        }
        //Debug.Log("isMidBoosting");
        totalDrift = 0;
    }

    public void turboBoost()
    {

        _rb.velocity = transform.forward * _rb.velocity.magnitude;
        //_rb.velocity = _rb.velocity - transform.right; //make sure the players drift is canceled out and they travel straight forwards
        foreach (AxleInfo axleInfo in _automobile.axleInfos)
        {
            _rb.AddForceAtPosition(transform.forward * (turboBoostSpeed), axleInfo.leftWheel.transform.position);
            _rb.AddForceAtPosition(transform.forward * (turboBoostSpeed), axleInfo.rightWheel.transform.position);
        }
        boostFXPos.position = transform.position;
        if (!boostFX.isPlaying)
        {
            if (!boostFX.enableEmission)
            {
                boostFX.enableEmission = true;
            }
            boostFX.Play();
        }

        //Debug.Log("isTurboBoosting");
        totalDrift = 0;
    }


    public void FixedUpdate()
    {
        _automobile.Drive(_dirInput);

        //just a small check to see if the vehicle is flipped over
        if ((Math.Abs(transform.eulerAngles.x) >= 90 && Math.Abs(transform.eulerAngles.x) <= 270) ||
            (Math.Abs(transform.eulerAngles.z) >= 90 && Math.Abs(transform.eulerAngles.z) <= 270))
        {
            isFlippedOver = true;
            Explode();
        }


        //Drifting stuff
        calculateDrift();

        if (_dirInput.x == 0 && totalDrift >= turboBoostThreshold)
            turboBoost();

        else if (_dirInput.x == 0 && totalDrift >= midBoostThreshold)
            midBoost();

        // else if (_dirInput.x == 0 && totalDrift >= miniBoostThreshold)
             // miniBoost();


        //Debug.Log(drift);
    }
}