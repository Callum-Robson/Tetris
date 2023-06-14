using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public BlockData blockData;
    private Vector2 currentMin_XY;
    private Vector2 currentMax_XY;
    private Vector2[] currentSubBlockCoordinates = new Vector2[5];

    public Vector2 CurrentMin_XY { get { return currentMin_XY; } }
    public Vector2 CurrentMax_XY { get { return currentMax_XY; } }
    public Vector2[] CurrentSubBlockCoordinates { get { return currentSubBlockCoordinates; } }


    public BlockManager blockManager;
    public Transform[] subBlocks = new Transform[5];

    public List<int> rowsOccupied = new List<int>();
    public List<int> columnsOccupied = new List<int>();

    public bool stopped = false;

    private void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        currentMin_XY = blockData.min_XY;
        currentMax_XY = blockData.max_XY;
        currentSubBlockCoordinates = blockData.subBlockCoordinates;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= blockManager.bounds.min.y - currentMin_XY.y && !stopped)
        {
            stopped = true;
            SnapToGrid();
            Vector3 ySnap = transform.position;
            ySnap.y = blockManager.bounds.min.y - currentMin_XY.y;
            //transform.position = ySnap;
            blockManager.ResetKeys();
        }
    }

    public void UpdatePositionData()
    {
        for (int i = 0; i < 5; i++)
        {
            currentSubBlockCoordinates[i] = subBlocks[i].transform.position;
        }

        // Bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less) will always be greater than Bounds.min.x
        currentMin_XY.x = blockManager.bounds.min.x - blockData.min_XY.x;
        currentMax_XY.x = blockManager.bounds.max.x - blockData.max_XY.x;
        Debug.Log(name + " currentMin = " + currentMin_XY + " , currentMax = " + currentMax_XY);
    }

    public void SnapToGrid()
    {
        //Get Position;
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;

        // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
        if (!blockManager.DivisibleByHalfAndNotOne(transform.position.x))
        {
            //Get offset from nearest 0.5
            float xOffset = xPosition > 0 ? (xPosition % 0.5f) : (xPosition % -0.5f);

            //Round down if offset less than 0.25
            if (Mathf.Abs(xOffset) < 0.25f)
            {
                // Operation will always reduce absolute value, so should never result in block moving out of bounds unless it already was.
                xPosition -= xOffset;
            }
            //Round up if offset greater than 0.25
            else if (Mathf.Abs(xOffset) > 0.25f)
            {
                // Round down absolute value
                xPosition -= xOffset;
                // If rounding "up" for negative number, subtract 0.5, if rounding up for position, add 0.5
                float roundingValue = xOffset < 0 ? -0.5f : 0.5f;
                // If xPosition is positive and xPosition rounded up is less than max position, make it so
                if (xPosition > 0 && xPosition + roundingValue <= currentMax_XY.x)
                {
                    xPosition += roundingValue;
                }
                // If xPosition is negative and xPosition rounded "up" is greater than min position, make it so
                else if (xPosition < 0 && xPosition + roundingValue >= currentMin_XY.x)
                {
                    xPosition += roundingValue;
                }
            }
        }

        if (!blockManager.DivisibleByHalfAndNotOne(transform.position.y))
        {
            //Get offset from nearest 0.5
            float yOffset = yPosition > 0 ? (yPosition % 0.5f) : (yPosition % -0.5f);

            //Round down if offset less than 0.25
            if (Mathf.Abs(yOffset) < 0.25f)
            {
                // Operation will always reduce absolute value, so should never result in block moving out of bounds unless it already was.
                yPosition -= yOffset;
            }
            //Round up if offset greater than 0.25
            else if (Mathf.Abs(yOffset) > 0.25f)
            {
                // Round down absolute value
                yPosition -= yOffset;
                // If rounding "up" for negative number, subtract 0.5, if rounding up for position, add 0.5
                float roundingValue = yOffset < 0 ? -0.5f : 0.5f;
                // If yPosition is positive and yPosition rounded up is less than max position, make it so
                if (yPosition > 0 && yPosition + roundingValue <= currentMax_XY.y)
                {
                    yPosition += roundingValue;
                }
                // If yPosition is negative and yPosition rounded "up" is greater than min position, make it so
                else if (yPosition < 0 && yPosition + roundingValue >= currentMin_XY.y)
                {
                    yPosition += roundingValue;
                }
            }
        }
        //Apply corrected position
        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
    }
}
