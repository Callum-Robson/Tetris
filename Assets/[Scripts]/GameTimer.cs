using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private static float fallTimer;
    private static float tickTimer;
    private NewManager pManager;

    private void Start()
    {
        pManager = FindObjectOfType<NewManager>();
    }

    public void WaitForTimer()
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.Timer)
        {
            bool stateChanged = false;
            InputManager.fallTriggered = false;
            tickTimer += Time.deltaTime;
            fallTimer += Time.deltaTime;

            if (tickTimer >= 0.1f && !stateChanged)
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
            if (fallTimer >= 0.5f && !stateChanged)
            {
                fallTimer = 0;
                stateChanged = true;
                InputManager.fallTriggered = true;

                // Move to wait state
                pManager.CheckCollision();
                GameplayStateMachine.NextState();
                //GameplayStateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            }
        }
    }
}
