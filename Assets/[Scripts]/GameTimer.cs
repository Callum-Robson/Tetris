using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float speedFactor = 1;
    private static float fallTimer;
    private static float tickTimer;
    private NewManager pManager;
    public bool fallTriggered = false;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        pManager = FindObjectOfType<NewManager>();
    }

    public IEnumerator Timer()
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.Timer)
        {
            bool stateChanged = false;
            InputManager.fallTriggered = false;
            tickTimer += Time.deltaTime;
            fallTimer += Time.deltaTime;

            if (fallTimer >= 0.5f / speedFactor && !stateChanged)
            {
                fallTimer = 0;
                stateChanged = true;
                InputManager.fallTriggered = true;
                InputManager.inputType = InputType.fall;
                // Move to wait state
                pManager.CheckCollision();
                GameplayStateMachine.NextState();
                //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            }
            else if (tickTimer >= 0.1f / speedFactor && !stateChanged)
            {
                tickTimer = 0;
                if (InputManager.inputX != 0 || InputManager.inputY != 0 || InputManager.rotationTriggered)
                {
                    stateChanged = true;

                    // Move to wait state
                    pManager.CheckCollision();
                    GameplayStateMachine.NextState();
                    //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
                }
            }

        }
        yield return null;
    }

    public void WaitForTimer()
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.Timer && NewManager.spawnRequired == false)
        {
            bool stateChanged = false;
            InputManager.fallTriggered = false;
            tickTimer += Time.deltaTime;
            fallTimer += Time.deltaTime;


            if (fallTimer >= 0.5f / speedFactor && !stateChanged)
            {
                fallTimer = 0;
                stateChanged = true;
                InputManager.fallTriggered = true;
                InputManager.inputType = InputType.fall;
                // Move to wait state
                pManager.CheckCollision();
                GameplayStateMachine.NextState();
                //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            }
            if (tickTimer >= 0.1f / speedFactor && !stateChanged)
            {
                playerController.Move();
                tickTimer = 0;
                if (InputManager.inputX != 0 || InputManager.inputY != 0 || InputManager.rotationTriggered)
                {
                    stateChanged = true;
                    if (InputManager.rotationTriggered)
                    {
                        InputManager.inputType = InputType.rotate;
                        InputManager.rotationTriggered = false;
                    }
                    // Move to wait state
                    pManager.CheckCollision();
                    GameplayStateMachine.NextState();
                    //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
                }
            }

            //if (tickTimer >= 0.1f && !stateChanged)
            //{
            //    tickTimer = 0;
            //    if (InputManager.inputX != 0 || InputManager.inputY != 0 || InputManager.rotationTriggered)
            //    {
            //        stateChanged = true;
            //
            //        // Move to wait state
            //        pManager.CheckCollision();
            //        GameplayStateMachine.NextState();
            //        //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            //    }
            //}
            //if (fallTimer >= 0.5f && !stateChanged)
            //{
            //    fallTimer = 0;
            //    stateChanged = true;
            //    InputManager.fallTriggered = true;
            //
            //    // Move to wait state
            //    pManager.CheckCollision();
            //    GameplayStateMachine.NextState();
            //    //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            //}
        }
        else if (NewManager.spawnRequired && !NewManager.waitingOnLineClear && GameplayStateMachine.CurrentState != GameplayStateMachine.States.WaitForLineClear)
        {
            GameplayStateMachine.SetState(GameplayStateMachine.States.Spawn);
        }
    }
}
