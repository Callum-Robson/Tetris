using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TheStateMachine : MonoBehaviour
{
    public enum GameplayState
    {
        Start,
        WaitingForSpawn,            // Transitions to WaitingForTimer (Maybe unnecessary)
        WaitingForTimer,            // Transitions to CheckingCollision
        CheckingCollision,          // Transitions to UpdatingOther Blocks
        WaitingOnCollisionCheck,    // Wait until CollisionCheck is done
        UpdatingOtherBlocks,        // Transitions to WaitingOnOthersBlocks
        WaitingOnOtherUpdate        // Wait until Other Blocks are updated
    }

    private GameplayState currentState;
    public GameplayState CurrentState { get { return currentState; } }

    private BlockManager blockManager;
    public TMP_Text lastStateText;
    public TMP_Text currentStateText;

    // Start is called before the first frame update
    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        SetState(GameplayState.Start);
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
            case GameplayState.UpdatingOtherBlocks:
                blockManager.UpdateOtherBlocks();
                break;
            case GameplayState.WaitingOnOtherUpdate:
                break;
            default:
                break;
        }
    }

    public void NextState()
    {
        switch (currentState)
        {
            case GameplayState.Start:
                break;
            case GameplayState.WaitingForSpawn:
                currentState = GameplayState.WaitingForTimer;
                lastStateText.text = "Waiting For Spawn";
                break;
            case GameplayState.WaitingForTimer:
                lastStateText.text = "Waiting For Timer";
                break;
            case GameplayState.CheckingCollision:
                currentState = GameplayState.WaitingOnCollisionCheck;
                lastStateText.text = "Check Collision";
                break;
            case GameplayState.WaitingOnCollisionCheck:
                currentState = GameplayState.UpdatingOtherBlocks;
                lastStateText.text = "Waiting on Collision Check";
                break;
            case (GameplayState.UpdatingOtherBlocks):
                currentState = GameplayState.WaitingOnOtherUpdate;
                lastStateText.text = "Updating Other Blocks";
                break;
            case (GameplayState.WaitingOnOtherUpdate):
                currentState = GameplayState.WaitingForSpawn;
                lastStateText.text = "Waiting On Other Update";
                break;
            default:
                break;
        }
    }

    public void SetState(GameplayState newState)
    {
        // Exit the current state
        switch (currentState)
        {
            case GameplayState.Start:
                break;
            case GameplayState.WaitingForSpawn:
                lastStateText.text = "Waiting For Spawn";
                break;
            case GameplayState.WaitingForTimer:
                lastStateText.text = "Waiting For Timer";
                break;
            case GameplayState.CheckingCollision:
                lastStateText.text = "Check Collision";
                break;
            case GameplayState.WaitingOnCollisionCheck:
                lastStateText.text = "Waiting on Collision Check";
                break;
            case (GameplayState.UpdatingOtherBlocks):
                lastStateText.text = "Updating Other Blocks";
                break;
            case (GameplayState.WaitingOnOtherUpdate):
                lastStateText.text = "Waiting On Other Update";
                break;
            default:
                break;
        }
        // Enter the new state
        switch (newState)
        {
            case GameplayState.Start:
                break;
            case GameplayState.WaitingForSpawn:
                currentStateText.text = "Waiting For Spawn";
                break;
            case GameplayState.WaitingForTimer:
                currentStateText.text = "Waiting For Timer";
                break;
            case GameplayState.CheckingCollision:
                currentStateText.text = "Check Collision";
                break;
            case GameplayState.WaitingOnCollisionCheck:
                currentStateText.text = "Waiting on Collision Check";
                break;
            case (GameplayState.UpdatingOtherBlocks):
                currentStateText.text = "Updating Other Blocks";
                break;
            case (GameplayState.WaitingOnOtherUpdate):
                currentStateText.text = "Waiting On Other Update";
                break;
            default:
                break;
        }
        currentState = newState;
    }

}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//
//public class TheStateMachine : MonoBehaviour
//{
//    public enum GameplayState
//    {
//        Start,
//        WaitingForSpawn,            // Transitions to WaitingForTimer (Maybe unnecessary)
//        WaitingForTimer,            // Transitions to CheckingCollision
//        CheckingCollision,          // Transitions to UpdatingActiveBlock
//        WaitingOnCollisionCheck,    // Wait until CollisionCheck is done
//        UpdatingActiveBlock,        // Transitions to UpdatingOtherBlocks
//        WaitingOnActiveUpdate,      // Wait until Acitve Block is updated
//        UpdatingOtherBlocks,        // Transitions to WaitingForTimer
//        WaitingOnOtherUpdate        // Wait until Other Blocks are updated
//    }
//
//    private GameplayState currentState;
//    public GameplayState CurrentState { get { return currentState; } }
//
//    private BlockManager blockManager;
//    [SerializeField]
//    private TMP_Text lastStateText;
//    [SerializeField]
//    private TMP_Text currentStateText;
//
//    // Start is called before the first frame update
//    void Start()
//    {
//        blockManager = FindObjectOfType<BlockManager>();
//        blockManager.StateMachine = this;
//        SetState(GameplayState.WaitingForSpawn);
//
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//        // Check for state-specific input or logic
//        switch (currentState)
//        {
//            case GameplayState.Start:
//                break;
//            case GameplayState.WaitingForSpawn:
//                blockManager.SpawnBlock();
//                break;
//            case GameplayState.WaitingForTimer:
//                blockManager.WaitForTimer();
//                break;
//            case GameplayState.CheckingCollision:
//                blockManager.CheckCollision();
//                break;
//            case GameplayState.WaitingOnCollisionCheck:
//                break;
//            case GameplayState.UpdatingActiveBlock:
//                break;
//            case GameplayState.WaitingOnActiveUpdate:
//                break;
//            case GameplayState.UpdatingOtherBlocks:
//                blockManager.UpdateOtherBlocks();
//                break;
//            case GameplayState.WaitingOnOtherUpdate:
//                break;
//        }
//    }
//
//    public void SetState(GameplayState newState)
//    {
//        // Exit the current state
//        switch (currentState)
//        {
//            case GameplayState.Start:
//                break;
//            case GameplayState.WaitingForSpawn:
//                blockManager.SpawnBlock();
//                lastStateText.text = "Waiting For Spawn";
//                break;
//            case GameplayState.WaitingForTimer:
//                blockManager.WaitForTimer();
//                lastStateText.text = "Waiting For Timer";
//                break;
//            case GameplayState.CheckingCollision:
//                blockManager.CheckCollision();
//                lastStateText.text = "Check Collision";
//                break;
//            case GameplayState.WaitingOnCollisionCheck:
//                lastStateText.text = "Waiting on Collision Check";
//                break;
//            case (GameplayState.UpdatingActiveBlock):
//                lastStateText.text = "Updating Active Block";
//                break;
//            case (GameplayState.WaitingOnActiveUpdate):
//                lastStateText.text = "Waiting On Active Update";
//                break;
//            case (GameplayState.UpdatingOtherBlocks):
//                lastStateText.text = "Updating Other Blocks";
//                break;
//            case (GameplayState.WaitingOnOtherUpdate):
//                lastStateText.text = "Updating On Other Update";
//                break;
//        }
//        // Enter the new state
//        switch (newState)
//        {
//            case GameplayState.Start:
//                break;
//            case GameplayState.WaitingForSpawn:
//                blockManager.SpawnBlock();
//                currentStateText.text = "Waiting For Spawn";
//                break;
//            case GameplayState.WaitingForTimer:
//                blockManager.WaitForTimer();
//                currentStateText.text = "Waiting For Timer";
//                break;
//            case GameplayState.CheckingCollision:
//                blockManager.CheckCollision();
//                currentStateText.text = "Check Collision";
//                break;
//            case GameplayState.WaitingOnCollisionCheck:
//                currentStateText.text = "Waiting on Collision Check";
//                break;
//            case (GameplayState.UpdatingActiveBlock):
//                currentStateText.text = "Updating Active Block";
//                break;
//            case (GameplayState.WaitingOnActiveUpdate):
//                currentStateText.text = "Waiting On Active Update";
//                break;
//            case (GameplayState.UpdatingOtherBlocks):
//                currentStateText.text = "Updating Other Blocks";
//                break;
//            case (GameplayState.WaitingOnOtherUpdate):
//                currentStateText.text = "Updating On Other Update";
//                break;
//        }
//        currentState = newState;
//    }
//}
//