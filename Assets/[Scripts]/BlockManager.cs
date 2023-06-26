using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    public float keyHoldThresholdTime = 1.0f;
    public float fastBlockSpeed = 1.0f;
    private bool keyHeld;
    private bool keyPressStarted;
    private bool canMoveRight, canMoveLeft;
    private float inputX = 0;
    private float inputY = 0;
    private float keyTimer = 0;

    public float secondCounter;
    public float halfSecondCounter;
    public float quarterSecondCounter;
    public float eigthSecondCounter;
    public float elapsedTime;

    public BlockBehaviour[] blockPrefabs;
    public List<BlockBehaviour> activeBlocks = new List<BlockBehaviour>();
    private BlockBehaviour fallingBlock;

    public Rect bounds;

    public Grid theGrid;

    private TheStateMachine stateMachine;
    public TheStateMachine StateMachine { get { return stateMachine; } }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = FindObjectOfType<TheStateMachine>();
        theGrid = FindObjectOfType<Grid>();
        bounds.width -= 1.0f;
        bounds.height -= 1.0f;
        bounds.center = new Vector2(11.5f, 19.5f);
        Debug.Log("Bounds.Min = " + bounds.min);
        Debug.Log("Bound.Max = " + bounds.max);

        SpawnBlock();
    }

    public void UpdateOtherBlocks()
    {
        stateMachine.SetState(TheStateMachine.GameplayState.WaitingOnOtherUpdate);
        // Check for filled lines

        // If filled lines were found and removed, update any blocks which can now fall

        // TODO: Either 1. Immediately update newly falling blocks to their end position (Easy Option) or 2. Lerp them or something which will take time and requiring more functions or coroutines or whatever (Hard Option)
        if (fallingBlock.stopped)
        {
            stateMachine.SetState(TheStateMachine.GameplayState.WaitingForSpawn);
        }
        else
        {
            stateMachine.SetState(TheStateMachine.GameplayState.WaitingForTimer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Make sure this state is only active if falling block has not stopped
        if(stateMachine.CurrentState == TheStateMachine.GameplayState.WaitingForTimer)
        {
            //A Input
            //A-1 Keys - Pressed
            if (Input.GetKeyDown(KeyCode.A) && fallingBlock.canMoveLeft)
            {
                keyPressStarted = true;
                inputX = -1;
            }
            else if (Input.GetKeyDown(KeyCode.D) && fallingBlock.canMoveRight)
            {
                keyPressStarted = true;
                inputX = 1;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                keyPressStarted = true;
                inputY = -1;
            }
            else if (!keyHeld)
            {
                inputX = 0;
                inputY = 0;
            }

            //B-2 Keys - Released
            if ((Input.GetKeyUp(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) || (Input.GetKeyUp(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
                || (Input.GetKeyUp(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)))
            {
                Debug.Log("Key up triggered");
                inputX = 0;
                inputY = 0;
                keyPressStarted = false;
                keyTimer = 0;
                keyHeld = false;

                //fallingBlock.NewSnapToGrid();  // SnapToGrid();
            }
            //B-3 Keys - Switched
            if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                inputX = 1;
                inputY = 0;
            }
            else if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                inputX = 0;
                inputY = 1;
            }
            if (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                inputX = -1;
                inputY = 0;
            }
            else if (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                inputX = 0;
                inputY = -1;
            }
            if (Input.GetKeyUp(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                inputY = 0;
                inputX = -1;
            }
            else if (Input.GetKeyUp(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                inputY = 0;
                inputX = 1;
            }

            //B-4 Keys - Held 
            if (keyPressStarted)
            {
                keyTimer += Time.deltaTime;
                if (keyTimer > keyHoldThresholdTime)
                {
                    keyHeld = true;
                    if (Input.GetKey(KeyCode.A) && fallingBlock.canMoveLeft)
                        inputX = -1;  
                    else if (Input.GetKey(KeyCode.D) && fallingBlock.canMoveRight)
                        inputX = 1;
                    else if (Input.GetKey(KeyCode.S))
                        inputY = -1;

                    if (!fallingBlock.canMoveRight && inputX > 0)
                    {
                        ResetKeys();
                    }
                    if (!fallingBlock.canMoveLeft && inputX < 0)
                    {
                        ResetKeys();
                    }
                }
            }

            //C-1 Movement - Input movement
            //if (!fallingBlock.stopped)
            //{   //If falling block has not stopped, apply input movement.

                if (keyPressStarted && !keyHeld)
                {
                    //Debug.Log("Key press movement");
                    //if (inputX != 0)
                    //    fallingBlock.CollisionCheck(MoveType.INPUT, true, inputX); //fallingBlock.InputMovement(true, inputX); 
                    //else if (inputY != 0)
                    //    fallingBlock.CollisionCheck(MoveType.INPUT, false, inputY); //fallingBlock.InputMovement(false, inputY);
                }
            //}



            //C-3 Movement - Rotation Input
            if (Input.GetKeyDown(KeyCode.E))
            {
                fallingBlock.Rotate();
            }


            ////D-1 Timers
            //if (eigthSecondCounter >= 0.125f)
            //{
            //    if (keyHeld)
            //    {
            //       // Debug.Log("Key held movement");
            //       // if (inputX != 0)
            //       //     fallingBlock.CollisionCheck(MoveType.INPUT, true, inputX);  //fallingBlock.InputMovement(true, inputX);
            //       // else if (inputY != 0)
            //       //     fallingBlock.CollisionCheck(MoveType.INPUT, false, inputY); //fallingBlock.InputMovement(false, inputY);
            //    }
            //    //CheckForFilledRow();
            //    //fallingBlock.CollisionCheck();
            //    eigthSecondCounter = 0.0f;
            //}
            //
            //if (quarterSecondCounter >= 0.25f)
            //{
            //
            //    quarterSecondCounter = 0.0f;
            //}
            //if (halfSecondCounter >= 0.5f)
            //{
            //    if (inputY >= 0)
            //        //fallingBlock.CollisionCheck(MoveType.FALL, false, 0.0f);    //fallingBlock.Fall();
            //
            //    //fallingBlock.CollisionCheck();
            //    halfSecondCounter = 0.0f;
            //}
            //
            //if (secondCounter >= 1.00f)// && !fallingBlock.stopped)
            //{
            //    elapsedTime += secondCounter;
            //    secondCounter = 0.0f;
            //    //fallingBlock.transform.position += Vector3.down;
            //    //fallingBlock.UpdatePositionData();
            //}

        }

        //E-1 Spawn Blocks
        //if (fallingBlock.stopped)
        //{   // Remove fallingBlock reference, spawn a new Block.
        //    fallingBlock = null;
        //    SpawnBlock();
        //}
    }

    public void SpawnBlock()
    {
        // Pick random index for random Pentomino
        int randomBlock = Random.Range(0, 12);
        // Pick random x value for spawn position
        float randomX = (int)Random.Range(bounds.xMin, bounds.xMax + 1);

        // Add or Subtract 0.5 from generated integer value
        //if (Random.Range(0,2) == 0)
        //    randomX += 0.5f;
        //else
        //    randomX -= 0.5f;

        // Calculate minimum and maxium x position selected block can occupy while remaining in bounds.
        float minPositionX = -blockPrefabs[randomBlock].blockData.min_XY.x + bounds.min.x;
        float maxPositionX = bounds.max.x - blockPrefabs[randomBlock].blockData.max_XY.x;

        // Y Position for spawn     ( Should this be above edge of screen or should block spawn completely on-screen? )
        float ySpawn = bounds.max.y - blockPrefabs[randomBlock].blockData.max_XY.y;

        // If randomX is out of bounds, set it to minPositionX or maxPositionX, whichever is closest.
        if (minPositionX > randomX)
            randomX = minPositionX;
        else if (maxPositionX < randomX)
            randomX = maxPositionX;


        Vector3 spawnPosition = new Vector3(randomX, ySpawn, 0);
        
        // Instantiate selected block prefab at generated position, with default rotation, as fallingBlock. Add to activeBlocks.
        fallingBlock = Instantiate(blockPrefabs[randomBlock], spawnPosition, blockPrefabs[randomBlock].transform.rotation);
        activeBlocks.Add(fallingBlock);

        
        stateMachine.SetState(TheStateMachine.GameplayState.WaitingForTimer);
    }

    public void WaitForTimer()
    {
        bool stateChanged = false;
        eigthSecondCounter += Time.deltaTime;
        quarterSecondCounter += Time.deltaTime;
        halfSecondCounter += Time.deltaTime;
        secondCounter += Time.deltaTime;

        //D-1 Timers
        if (eigthSecondCounter >= 0.125f && !stateChanged)
        {
            if (keyHeld)
            {
                stateMachine.SetState(TheStateMachine.GameplayState.CheckingCollision);
                stateChanged = true;
            }
            eigthSecondCounter = 0.0f;
        }

        if (halfSecondCounter >= 0.5f && !stateChanged)
        {
            halfSecondCounter = 0.0f;
            stateMachine.SetState(TheStateMachine.GameplayState.CheckingCollision);
            stateChanged = true;
        }

        if (secondCounter >= 1.00f)
        {
            elapsedTime += secondCounter;
            secondCounter = 0.0f;
        }
    }

    public void CheckCollision()
    {
        stateMachine.SetState(TheStateMachine.GameplayState.WaitingOnCollisionCheck);
        if (inputX != 0)
        {
            fallingBlock.CollisionCheck(MoveType.INPUT, true, inputX);
        }
        if (inputY < 0)
        {
            fallingBlock.CollisionCheck(MoveType.INPUT, false, inputY);
        }
        else if (inputY == 0)
        {
            fallingBlock.CollisionCheck(MoveType.FALL, false, 0.0f);
        }
    }

    public void ResetKeys()
    {
        inputX = 0;
        inputY = 0;
        keyHeld = false;
        keyPressStarted = false;
        keyTimer = 0;
    }

    public bool DivisibleByHalfAndNotOne(float value)
    {
        if ((value / 0.5f) % 1 == 0 && value % 1 != 0)
            return true;
        else
            return false;
    }

    private void CheckForFilledRow()
    {
        for (int i = 0; i < 4; i++) // Grid.cells.GetLength(1); i++)
        {
            bool emptyCellFound = false;
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                if (!Grid.cells[i2,i].GetFilledState())
                {
                    emptyCellFound = true;
                    Debug.Log("Row #" + i + " has not been filled");
                    break;
                }
            }
            if (!emptyCellFound)
            {
                //Delete these subBlocks
                Debug.Log("Row #" + i + " has been filled");
            }
        }
    }

}


////C-2 Movement - Snap back inside bounds
//Vector2 tempPosition = fallingBlock.transform.position;
//float tempX = fallingBlock.transform.position.x;
//float tempY = fallingBlock.transform.position.y;
//
//
//for (int i = 0; i < 5; i++)
//{
//    if (!bounds.Contains(fallingBlock.subBlocks[i].transform.position))
//    {
//        //New
//        tempX = fallingBlock.subBlocks[i].transform.position.x;
//        //End of new
//        if (tempX < bounds.min.x)
//        {
//            ResetKeys();
//
//            tempPosition.x = Mathf.Round(tempPosition.x);
//            if(tempPosition.x < bounds.min.x)
//            {
//                tempPosition.x++;
//            }
//            fallingBlock.transform.position = tempPosition;
//        }
//        else if (tempX > bounds.max.x)
//        {
//            ResetKeys();
//
//            tempPosition.x = Mathf.Round(tempPosition.x);
//            if(tempPosition.x > bounds.min.x)
//            {
//                tempPosition.x--;
//            }
//            fallingBlock.transform.position = tempPosition;
//        }
//        i = 6;
//    }
//}




