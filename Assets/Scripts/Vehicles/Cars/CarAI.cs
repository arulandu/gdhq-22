using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{

    //determine where the car should steer to depending on its position in the map
    public static Vector2 carAutopilot (float xPos, float yPos,   int [ , ] cityMap) {
        return new Vector2 (0, 0);
    }
}
