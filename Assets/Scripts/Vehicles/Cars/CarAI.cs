using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    GameObject carObject;
    CarController carController;
    static int [ , ] cityMap;

    //determine where the car should steer to depending on its position in the map
    

    //turn to a forward facing direction (not relative to the car)
    void turnForwards() {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //turn to a left facing direction (not relative to the car)
    void turnLeft() {
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    //turn to a backwards facing direction (not relative to the car)
    void turnBackwards() {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    //turn to a right facing direction (not relative to the car)
    void turnRight() {
        transform.rotation = Quaternion.Euler(0, 270, 0);
    }




    public static void setCityMap(int[ , ] cityMap_) {
        cityMap = cityMap_;
    }
}   
