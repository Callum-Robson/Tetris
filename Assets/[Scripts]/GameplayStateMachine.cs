using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayStateMachine : MonoBehaviour
{
    public enum States
    {
        Start,
        Spawn,
        Timer,
        Wait,
        WaitForLineClear
    }


    private NewManager pManager;
    private GameTimer gameTimer;
    private static States currentState = States.Start;

    public States currentStateDisplay = States.Start;
    public static States CurrentState { get { return currentState; } }

    public static bool stateMethodCalled = false;


    public static void NextState()
    {
        switch (currentState)
        {
            case States.Start:
                currentState = States.Spawn;
                break;
            case States.Spawn:
                currentState = States.Timer;
                stateMethodCalled = false;
                break;
            case States.Timer:
                currentState = States.Wait;
                break;
            case States.Wait:
                currentState = States.Timer;
                break;
            case States.WaitForLineClear:
                break;
        }
    }

    public static void SetState(States newState)
    {
        currentState = newState;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();
        pManager = FindObjectOfType<NewManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentStateDisplay = currentState;
        if (currentState == States.Spawn && !stateMethodCalled)
        {
            stateMethodCalled = true;
            pManager.SpawnBlock();
        }
        if (currentState == States.Timer)
        {
            gameTimer.WaitForTimer();
        }
        //switch (currentState)
        //{
        //    case States.Start:
        //        Debug.Log(currentState + " entered at " + Time.time);
        //        break;
        //    case States.Spawn:
        //        Debug.Log(currentState + " entered at " + Time.time);
        //        pManager.SpawnBlock();
        //        break;
        //    case States.Timer:
        //        Debug.Log(currentState + " entered at " + Time.time);
        //        break;
        //    case States.CollisionCheck:
        //        Debug.Log(currentState + " entered at " + Time.time);
        //        pManager.CheckCollision();
        //        break;
        //    case States.Wait:
        //        Debug.Log(currentState + " entered at " + Time.time);
        //        break;
        //}
    }
}
