using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStateMachine : MonoBehaviour
{
    public enum GameplayState
    {
        Start,
        WaitingForSpawn,            // Transitions to WaitingForTimer (Maybe unnecessary)
        WaitingForTimer,            // Transitions to CheckingCollision
        CheckingCollision,          // Transitions to UpdatingActiveBlock
        WaitingOnCollisionCheck,    // Wait until CollisionCheck is done
        UpdatingActiveBlock,        // Transitions to UpdatingOtherBlocks
        WaitingOnActiveUpdate,      // Wait until Acitve Block is updated
        UpdatingOtherBlocks,        // Transitions to WaitingForTimer
        WaitingOnOtherUpdate        // Wait until Other Blocks are updated
    }

    private GameplayState currentState;
    public GameplayState CurrentState { get { return currentState; } }

    private BlockManager blockManager;
    // Start is called before the first frame update
    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        SetState(GameplayState.WaitingForSpawn);

    }

    // Update is called once per frame
    void Update()
    {
        // Check for state-specific input or logic
        switch (currentState)
        {
            case GameplayState.Start:
                break;
            case GameplayState.WaitingForSpawn:
                blockManager.SpawnBlock();
                break;
            case GameplayState.WaitingForTimer:
                blockManager.WaitForTimer();
                break;
            case GameplayState.CheckingCollision:
                blockManager.CheckCollision();
                break;
            case GameplayState.WaitingOnCollisionCheck:
                break;
            case GameplayState.UpdatingActiveBlock:
                break;
            case GameplayState.WaitingOnActiveUpdate:
                break;
            case GameplayState.UpdatingOtherBlocks:
                blockManager.UpdateOtherBlocks();
                break;
            case GameplayState.WaitingOnOtherUpdate:
                break;
        }
    }

    public void SetState(GameplayState newState)
    {
        //// Exit the current state
        //switch (currentState)
        //{
        //    case (GameplayState.Start):
        //        break;
        //    case (GameplayState.WaitingForSpawn):
        //        break;
        //    case (GameplayState.WaitingForTimer):
        //        break;
        //    case (GameplayState.CheckingCollision):
        //        break;
        //    case (GameplayState.WaitingOnCollisionCheck):
        //        break;
        //    case (GameplayState.UpdatingActiveBlock):
        //        break;
        //    case (GameplayState.WaitingOnActiveUpdate):
        //        break;
        //    case (GameplayState.UpdatingOtherBlocks):
        //        break;
        //    case (GameplayState.WaitingOnOtherUpdate):
        //        break;
        //}
        //// Enter the new state
        //switch (newState)
        //{
        //    case (GameplayState.Start):
        //        break;
        //    case (GameplayState.WaitingForSpawn):
        //        break;
        //    case (GameplayState.WaitingForTimer):
        //        break;
        //    case (GameplayState.CheckingCollision):
        //        break;
        //    case (GameplayState.WaitingOnCollisionCheck):
        //        break;
        //    case (GameplayState.UpdatingActiveBlock):
        //        break;
        //    case (GameplayState.WaitingOnActiveUpdate):
        //        break;
        //    case (GameplayState.UpdatingOtherBlocks):
        //        break;
        //    case (GameplayState.WaitingOnOtherUpdate):
        //        break;
        //}
        currentState = newState;
    }
}
