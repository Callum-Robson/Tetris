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

    // Start is called before the first frame update
    void Start()
    {
        theGrid = FindObjectOfType<Grid>();
        bounds.width -= 1.0f;
        bounds.height -= 1.0f;
        bounds.center = Vector2.zero;
        Debug.Log("Bounds.Min = " + bounds.min);
        Debug.Log("Bound.Max = " + bounds.max);

        SpawnBlock();
    }

    // Update is called once per frame
    void Update()
    {
        eigthSecondCounter += Time.deltaTime;
        quarterSecondCounter += Time.deltaTime;
        halfSecondCounter += Time.deltaTime;
        secondCounter += Time.deltaTime;
        Debug.Log("secondCounter = " + secondCounter);
        //A. Bounds Checking
        //CheckBlockAgainstBounds();
        if (!fallingBlock.stopped)
        {
            fallingBlock.CheckBlockAgainstBounds();

            //B. Input
            //B-1 Keys - Pressed
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

                fallingBlock.NewSnapToGrid();  // SnapToGrid();
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
                        inputX = -1;    // -fastBlockSpeed * Time.deltaTime;
                    else if (Input.GetKey(KeyCode.D) && fallingBlock.canMoveRight)
                        inputX = 1;  //fastBlockSpeed * Time.deltaTime;
                    else if (Input.GetKey(KeyCode.S))
                        inputY = -1;    // -fastBlockSpeed * Time.deltaTime;

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
                    Debug.Log("Key press movement");
                    if (inputX != 0)
                        fallingBlock.InputMovement(true, inputX); //fallingBlock.transform.position += Vector3.right * inputX;
                    else if (inputY != 0)
                        fallingBlock.InputMovement(false, inputY); //fallingBlock.transform.position += Vector3.up * inputY;
                }
            //}



            //C-3 Movement - Rotation Input
            if (Input.GetKeyDown(KeyCode.E))
            {
                fallingBlock.Rotate();

                //fallingBlock.transform.Rotate(new Vector3(0, 0, 90));
                //for (int i = 0; i < 5; i++)
                //{
                //    if (!bounds.Contains(fallingBlock.subBlocks[i].transform.position))
                //    {
                //        fallingBlock.transform.Rotate(new Vector3(0, 0, -90));
                //        i = 6;
                //    }
                //}
                //fallingBlock.UpdatePositionData();
            }


            //D-1 Timers
            if (eigthSecondCounter >= 0.125f)
            {
                if (keyHeld)
                {
                    Debug.Log("Key held movement");
                    if (inputX != 0)
                        fallingBlock.InputMovement(true, inputX); //fallingBlock.transform.position += Vector3.right * inputX;
                    else if (inputY != 0)
                        fallingBlock.InputMovement(false, inputY); //fallingBlock.transform.position += Vector3.up * inputY;
                }
                eigthSecondCounter = 0.0f;
            }

            if (quarterSecondCounter >= 0.25f)
            {

                quarterSecondCounter = 0.0f;
            }
            if (halfSecondCounter >= 0.5f)
            {
                if (inputY >= 0)
                    fallingBlock.Fall();
                halfSecondCounter = 0.0f;
            }

            if (secondCounter >= 1.00f)// && !fallingBlock.stopped)
            {
                elapsedTime += secondCounter;
                secondCounter = 0.0f;
                //fallingBlock.transform.position += Vector3.down;
                //fallingBlock.UpdatePositionData();
            }

        }

        //E-1 Spawn Blocks
        if (fallingBlock.stopped)
        {   // Remove fallingBlock reference, spawn a new Block.
            fallingBlock = null;
            SpawnBlock();
        }
    }

    public void SpawnBlock()
    {
        // Pick random index for random Pentomino
        int randomBlock = Random.Range(0, 12);
        // Pick random x value for spawn position
        float randomX = (int)Random.Range(bounds.xMin, bounds.xMax + 1);

        // Add or Subtract 0.5 from generated integer value
        if (Random.Range(0,2) == 0)
            randomX += 0.5f;
        else
            randomX -= 0.5f;

        // Calculate minimum and maxium x position selected block can occupy while remaining in bounds.
        float minPositionX = -blockPrefabs[randomBlock].blockData.min_XY.x + bounds.min.x;
        float maxPositionX = bounds.max.x - blockPrefabs[randomBlock].blockData.max_XY.x;

        // Y Position for spawn     ( Should this be above edge of screen or should block spawn completely on-screen? )
        float ySpawn = bounds.max.y;

        // If randomX is out of bounds, set it to minPositionX or maxPositionX, whichever is closest.
        if (minPositionX > randomX)
            randomX = minPositionX;
        else if (maxPositionX < randomX)
            randomX = maxPositionX;


        Vector3 spawnPosition = new Vector3(randomX, ySpawn, 0);
        
        // Instantiate selected block prefab at generated position, with default rotation, as fallingBlock. Add to activeBlocks.
        fallingBlock = Instantiate(blockPrefabs[randomBlock], spawnPosition, blockPrefabs[randomBlock].transform.rotation);
        activeBlocks.Add(fallingBlock);
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


//Moved to BlockBehaviour
//public void SnapToGrid()
//{
//    // Snap Odd numbered blocks to int +/- 0.5
//    // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
//    if (!DivisibleByHalfAndNotOne(fallingBlock.transform.position.x))
//    {
//        //Get X Position;
//        float xPosition = fallingBlock.transform.position.x;
//
//        //Get offset from nearest 0.5
//        float xOffset = xPosition > 0 ? (xPosition % 0.5f) : (xPosition % -0.5f);
//
//        //Round down if offset less than 0.25
//        if (Mathf.Abs(xOffset) < 0.25f)
//        {
//            // Operation will always reduce absolute value, so should never result in block moving out of bounds unless it already was.
//            xPosition -= xOffset;
//        }
//        //Round up if offset greater than 0.25
//        else if (Mathf.Abs(xOffset) > 0.25f)
//        {
//            // Round down absolute value
//            xPosition -= xOffset;
//            // If rounding "up" for negative number, subtract 0.5, if rounding up for position, add 0.5
//            float roundingValue = xOffset < 0 ? -0.5f : 0.5f;
//            // If xPosition is positive and xPosition rounded up is less than max position, make it so
//            if (xPosition > 0 && xPosition + roundingValue <= fallingBlock.CurrentMax_XY.x)
//            {
//                xPosition += roundingValue;
//            }
//            // If xPosition is negative and xPosition rounded "up" is greater than min position, make it so
//            else if (xPosition < 0 && xPosition + roundingValue >= fallingBlock.CurrentMin_XY.x)
//            {
//                xPosition += roundingValue;
//            }
//        }
//        //Apply corrected position
//        fallingBlock.transform.position = new Vector3(xPosition, fallingBlock.transform.position.y, fallingBlock.transform.position.z);
//        Debug.Log("Snapped odd block,  New xPosition = " + xPosition);
//    }
//}

//Moved to BlockBehaviour
//public void CheckBlockAgainstBounds()
//{
//    // If position.x is greater than bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less), there is still room to move left
//    if (fallingBlock.transform.position.x > bounds.min.x - fallingBlock.blockData.min_XY.x)
//        canMoveLeft = true;
//    else
//        canMoveLeft = false;
//
//    // If fallingBlock position.x is less than bounds.max.x (11.5) minus fallingBlock's max.x (always 0 or more), there is still room to move right
//    if (fallingBlock.transform.position.x < bounds.max.x - fallingBlock.blockData.max_XY.x)
//        canMoveRight = true;
//    else
//        canMoveRight = false;
//
//}



//public void SnapToGridOld()
//{
//    //Snap even numbered blocks to int
//    if (fallingBlock.blockData.width % 2 == 0 && fallingBlock.transform.position.x % 2 != 0)
//    {
//        float xPosition = fallingBlock.transform.position.x;
//        Debug.Log("xPosition = " + xPosition);
//        float xOffset = (xPosition % 1);
//        Debug.Log("xOffset = " + xOffset);
//
//        // Round down
//        if (xOffset < 0.25f)
//        {
//            if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition -= xOffset;
//        }
//        //else if (xOffset > 0.25f && xOffset < 0.75f)
//        //{
//        //    xPosition -= xOffset;
//        //    xPosition += 0.5f;
//        //}
//
//        // Round up
//        else if (xOffset > 0.75f)
//        {
//            if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition -= xOffset;
//            if (xPosition + 1.0f < fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition += 1.0f;
//        }
//        Debug.Log("Snapped even block,  New xPosition = " + xPosition);
//        fallingBlock.transform.position = new Vector3(xPosition, fallingBlock.transform.position.y, fallingBlock.transform.position.z);
//    }
//    // Snap Odd numbered blocks to int +/- 0.5
//    // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
//    else if (fallingBlock.blockData.width % 2 != 0 && !DivisibleByHalfAndNotOne(fallingBlock.transform.position.x))
//    {
//        bool snapped = false;
//
//        //Get X Position;
//        float xPosition = fallingBlock.transform.position.x;
//        Debug.Log("xPosition = " + xPosition);
//
//        //Get difference between 
//        float xOffset = (xPosition % 0.5f);
//        Debug.Log("xOffset = " + xOffset);
//
//        if (xOffset < 0.25f)
//        {
//            if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition -= xOffset;
//        }
//        else if (xOffset > 0.25f)
//        {
//            if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition -= xOffset;
//            if (xPosition + 0.5f <= fallingBlock.blockData.maxAllowablePosition.x)
//                xPosition += 0.5f;
//        }
//
//        Debug.Log("Snapped odd block,  New xPosition = " + xPosition);
//        fallingBlock.transform.position = new Vector3(xPosition, fallingBlock.transform.position.y, fallingBlock.transform.position.z);
//    }
//}



//public void SpawnBlockWithOffset()
//{
//    //modify so odd and even dimensioned blocks are properly offset or not offset by 0.5
//    int randomBlock = Random.Range(0, 12);
//
//    float randomX = Random.Range(bounds.xMin, bounds.xMax + 1);
//
//    Debug.Log("randomX difference = " + (randomX - Mathf.Round(randomX)));
//
//    if (Mathf.Abs(randomX - Mathf.Round(randomX)) < 0.25)
//    {
//        randomX = Mathf.Round(randomX);
//    }
//    else
//    {
//        randomX = Mathf.Round(randomX);
//        randomX += 0.5f;
//    }
//
//    if (blockPrefabs[randomBlock].blockData.width % 2 == 0)
//    {
//        randomX -= (randomX % 1);
//    }
//
//    Vector2 minPosition = -blockPrefabs[randomBlock].blockData.maxAllowablePosition;
//    Vector2 maxPosition = blockPrefabs[randomBlock].blockData.maxAllowablePosition;
//
//    minPosition.x *= sizeFactorX;
//    minPosition.y *= sizeFactorY;
//
//    maxPosition.x *= sizeFactorX;
//    maxPosition.y *= sizeFactorY;
//
//    float ySpawn = maxPosition.y;
//
//    if (blockPrefabs[randomBlock].blockData.width % 2 != 0 && blockPrefabs[randomBlock].blockData.width != 1)
//    {
//        if (Random.Range(0, 2) == 0)
//            randomX -= 0.5f;
//        else
//            randomX += 0.5f;
//    }
//
//
//    if (minPosition.x > randomX)
//        randomX = minPosition.x;
//    else if (maxPosition.x < randomX)
//        randomX = maxPosition.x;
//
//    Vector3 spawnPosition = new Vector3(randomX, ySpawn, 0);
//
//    fallingBlock = Instantiate(blockPrefabs[randomBlock], spawnPosition, blockPrefabs[randomBlock].transform.rotation);
//    activeBlocks.Add(fallingBlock);
//
//
//}
