using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
    none,
    moveX,
    moveY,
    fall,
    rotate
}

public class InputManager : MonoBehaviour
{
    public static int inputX = 0;
    public static int inputY = 0;

    public static bool fallTriggered = false;
    public static bool rotationTriggered = false;

    public static InputType inputType = InputType.none;


    // Update is called once per frame
    void Update()
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.Timer)
        {
            inputX = (int)Input.GetAxisRaw("Horizontal");
            if (inputX != 0)
                inputType = InputType.moveX;

            if (Input.GetKey(KeyCode.S))
            {
                inputType = InputType.moveY;
                inputY = -1;
            }
            else
                inputY = 0;

            if (Input.GetKeyDown(KeyCode.E))
            {
                inputType = InputType.rotate;
                rotationTriggered = true;
            }


            if (inputX == 0 && inputY == 0 && fallTriggered == false)
            {
                inputType = InputType.none;
            }
        }
     
    }
}
