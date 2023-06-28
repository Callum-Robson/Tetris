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
        CollisionCheck,
        Wait
    }


    private PentominoManager pManager;
    private States currentState = States.Start;


    public States CurrentState { get { return currentState; } }


    public void NextState()
    {
        switch (currentState)
        {
            case States.Start:
                currentState = States.Spawn;
                break;
            case States.Spawn:
                currentState = States.Timer;
                break;
            case States.Timer:
                currentState = States.CollisionCheck;
                break;
            case States.CollisionCheck:
                currentState = States.Wait;
                break;
            case States.Wait:
                currentState = States.Timer;
                break;
        }
    }

    public void SetState(States newState)
    {
        currentState = newState;
    }

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PentominoManager>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case States.Start:
                Debug.Log(currentState + " entered at " + Time.time);
                break;
            case States.Spawn:
                Debug.Log(currentState + " entered at " + Time.time);
                pManager.SpawnBlock();
                break;
            case States.Timer:
                Debug.Log(currentState + " entered at " + Time.time);
                pManager.WaitForTimer();
                break;
            case States.CollisionCheck:
                Debug.Log(currentState + " entered at " + Time.time);
                pManager.CheckForCollision();
                break;
            case States.Wait:
                Debug.Log(currentState + " entered at " + Time.time);
                break;
        }
    }
}
