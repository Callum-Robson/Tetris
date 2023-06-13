using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public float keyHoldThresholdTime = 1.0f;
    public float fastBlockSpeed = 0.01f;
    private bool keyHeld;
    private bool keyPressStarted;
    private bool canMoveRight, canMoveLeft;
    private float x = 0;
    private float y = 0;
    private float keyTimer = 0;

    public float CameraSize = 5;
    public float sizeFactorX = 1;
    public float sizeFactorY = 1;
    public float secondCounter;
    public float elapsedTime;

    public BlockBehaviour[] blockPrefabs;
    public List<BlockBehaviour> activeBlocks = new List<BlockBehaviour>();
    private BlockBehaviour fallingBlock;

    public Rect bounds;

    // Start is called before the first frame update
    void Start()
    {
        //switch(CameraSize)
        //{
        //    case 5:
        //        sizeFactorX = 1;
        //        sizeFactorY = 1;
        //        break;
        //    case 10:
        //        sizeFactorX = 7;
        //        sizeFactorY = 5;
        //        break;
        //    case 20:

        //        break;
        //    case 40:

        //        break;
        //    default:
        //        break;
        //}
        bounds.width = 6 * sizeFactorX;
        bounds.height = 10 * sizeFactorY;
        bounds.center = Vector2.zero;
        Debug.Log("Bounds.Min = " + bounds.min);
        Debug.Log("Bound.Max = " + bounds.max);

        SpawnBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (fallingBlock.transform.position.x > -fallingBlock.blockData.maxAllowablePosition.x)
            canMoveLeft = true;
        else
            canMoveLeft = false;
        if (fallingBlock.transform.position.x < fallingBlock.blockData.maxAllowablePosition.x)
            canMoveRight = true;
        else
            canMoveRight = false;

        if (Input.GetKeyDown(KeyCode.A) && canMoveLeft)
        {
            keyPressStarted = true;
            x = -1;
            //fallingBlock.transform.position += Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && canMoveRight)
        {
            keyPressStarted = true;
            x = 1;
            //fallingBlock.transform.position += Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.S) && fallingBlock.transform.position.y > -fallingBlock.blockData.maxAllowablePosition.y)
        {
            keyPressStarted = true;
            y = -1;
        }
        else if (!keyHeld)
        {
            x = 0;
            y = 0;
        }
        if ((Input.GetKeyUp(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) || (Input.GetKeyUp(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S))
            || (Input.GetKeyUp(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)))
        {
            Debug.Log("Key up triggered");
            x = 0;
            y = 0;
            keyPressStarted = false;
            keyTimer = 0;
            keyHeld = false;

            //Snap even numbered blocks to int
            if (fallingBlock.blockData.width % 2 == 0 && fallingBlock.transform.position.x % 2 != 0)
            {
                float xPosition = fallingBlock.transform.position.x;
                Debug.Log("xPosition = " + xPosition);
                float xOffset = (xPosition % 1);
                Debug.Log("xOffset = " + xOffset);

                // Round down
                if (xOffset < 0.25f)
                {
                    if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition -= xOffset;
                }
                //else if (xOffset > 0.25f && xOffset < 0.75f)
                //{
                //    xPosition -= xOffset;
                //    xPosition += 0.5f;
                //}

                // Round up
                else if (xOffset > 0.75f)
                {
                    if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition -= xOffset;
                    if (xPosition + 1.0f < fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition += 1.0f;
                }
                Debug.Log("Snapped even block,  New xPosition = " + xPosition);
                fallingBlock.transform.position = new Vector3(xPosition, fallingBlock.transform.position.y, fallingBlock.transform.position.z);
            }
            // Snap Odd numbered blocks to int +/- 0.5
                // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
            else if (fallingBlock.blockData.width % 2 != 0 && !DivisibleByHalfAndNotOne(fallingBlock.transform.position.x))
            {
                bool snapped = false;

                //Get X Position;
                float xPosition = fallingBlock.transform.position.x;
                Debug.Log("xPosition = " + xPosition);

                //Get difference between 
                float xOffset = (xPosition % 0.5f);
                Debug.Log("xOffset = " + xOffset);

                if (xOffset < 0.25f)
                {
                    if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition -= xOffset;
                }
                else if (xOffset > 0.25f)
                {
                    if (xPosition - xOffset > -fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition -= xOffset;
                    if (xPosition + 0.5f <= fallingBlock.blockData.maxAllowablePosition.x)
                        xPosition += 0.5f;
                }

                Debug.Log("Snapped odd block,  New xPosition = " + xPosition);
                fallingBlock.transform.position = new Vector3(xPosition, fallingBlock.transform.position.y, fallingBlock.transform.position.z);
            }
        }

        if (keyPressStarted)
        {
            keyTimer += Time.deltaTime;
            if (keyTimer > keyHoldThresholdTime)
            {
                keyHeld = true;
                if (Input.GetKey(KeyCode.A) && canMoveLeft)
                    x = -fastBlockSpeed * Time.deltaTime;
                else if (Input.GetKey(KeyCode.D) && canMoveRight)
                    x = fastBlockSpeed * Time.deltaTime;
                else if (Input.GetKey(KeyCode.S))
                    y = -fastBlockSpeed * Time.deltaTime;

                if (!canMoveRight && x > 0)
                {
                    //x = 0;
                    ResetKeys();
                }
                if (!canMoveLeft && x < 0)
                {
                    ResetKeys();
                }
            }
        }


        if (!fallingBlock.stopped)
        {
            if (keyHeld)
            {
                Debug.Log("Key held movement");
                if (x != 0)
                    fallingBlock.transform.position += Vector3.right * x;
                else if (y != 0)
                    fallingBlock.transform.position += Vector3.up * y;
            }
            else if (keyPressStarted)
            {
                Debug.Log("Key press movement");
                if (x != 0)
                    fallingBlock.transform.position += Vector3.right * x;
                else if (y != 0)
                    fallingBlock.transform.position += Vector3.up * y;
            }
        }


        secondCounter += Time.deltaTime;

        if (secondCounter >= 0.50f && !fallingBlock.stopped)
        {
            elapsedTime += secondCounter;
            secondCounter = 0.0f;
            fallingBlock.transform.position += Vector3.down;
        }

        if (fallingBlock.stopped)
        {
            fallingBlock = null;
            SpawnBlock();
        }
    }

    public void SpawnBlock()
    {

        //modify so odd and even dimensioned blocks are properly offset or not offset by 0.5
        int randomBlock = Random.Range(0, 12);

        float randomX = Random.Range(bounds.xMin, bounds.xMax + 1);

        Debug.Log("randomX difference = " + (randomX - Mathf.Round(randomX)));

        if (Mathf.Abs( randomX - Mathf.Round(randomX)) < 0.25)
        {
            randomX = Mathf.Round(randomX);
        }
        else
        {
            randomX = Mathf.Round(randomX);
            randomX += 0.5f;
        }

        if (blockPrefabs[randomBlock].blockData.width % 2 == 0)
        {
            randomX -= (randomX % 1);
        }

        Vector2 minPosition = -blockPrefabs[randomBlock].blockData.maxAllowablePosition;
        Vector2 maxPosition = blockPrefabs[randomBlock].blockData.maxAllowablePosition;

        minPosition.x *= sizeFactorX;
        minPosition.y *= sizeFactorY;

        maxPosition.x *= sizeFactorX;
        maxPosition.y *= sizeFactorY;

        float ySpawn = maxPosition.y;

        if (blockPrefabs[randomBlock].blockData.width % 2 != 0 && blockPrefabs[randomBlock].blockData.width != 1)
        {
            if (Random.Range(0, 2) == 0)
                randomX -= 0.5f;
            else
                randomX += 0.5f;
        }


        if (minPosition.x > randomX)
            randomX = minPosition.x;
        else if (maxPosition.x < randomX)
            randomX = maxPosition.x;

        Vector3 spawnPosition = new Vector3(randomX, ySpawn, 0);

        fallingBlock = Instantiate(blockPrefabs[randomBlock], spawnPosition, blockPrefabs[randomBlock].transform.rotation);
        activeBlocks.Add(fallingBlock);

        
    }

    public bool DivisibleByHalfAndNotOne(float value)
    {
        if ((value / 0.5f) % 1 == 0 && value % 1 != 0)
            return true;
        else
            return false;
    }

    public void ResetKeys()
    {
        x = 0;
        y = 0;
        keyHeld = false;
        keyPressStarted = false;
        keyTimer = 0;
    }
}
