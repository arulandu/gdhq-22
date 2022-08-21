using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Manager : MonoBehaviour
{
    // commented out for smoother turning
    // public PlayerInput playerInput;
    //
    // public static InputAction verticalInput;
    // public static InputAction horizontalInput;
    //
    // //called when the player joins and initializes all of the necesary objects
    // public void OnPlayerJoined(PlayerInput playerInput_){
    //
    //     playerInput = playerInput_;
    //
    //     verticalInput = playerInput.actions["Vertical"];
    //     horizontalInput = playerInput.actions["Horizontal"];
    // }
    //
    // //keeps updating variables for inputs (might bind this to events in the future if we want "action" based controls instead of only movement)
    // void FixedUpdate(){
    //
    //     CarController.horizontalInput = horizontalInput.ReadValue<float>();
    //     CarController.verticalInput = verticalInput.ReadValue<float>();
    // }
}
