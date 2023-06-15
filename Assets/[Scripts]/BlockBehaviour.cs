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

    private int pivotPointGridColumn;
    private int pivotPointGridRow;
    private Vector2[] occupiedGridCells = new Vector2[5];

    public BlockManager blockManager;
    public Transform[] subBlocks = new Transform[5];

    public List<Vector2> occupiedAdjacentCells = new List<Vector2>();

    public List<int> rowsOccupied = new List<int>();
    public List<int> columnsOccupied = new List<int>();

    public bool stopped = false;
    public bool canMoveLeft, canMoveRight, canMoveDown;

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
        if (transform.position.y <= currentMin_XY.y && !stopped)
        {
            stopped = true;
            NewSnapToGrid();
            //SnapToGrid();
            //Vector3 ySnap = transform.position;
            //ySnap.y = blockManager.bounds.min.y - currentMin_XY.y;
            //transform.position = ySnap;
            blockManager.ResetKeys();
        }
        //currentMin_XY.x = blockManager.bounds.min.x - blockData.min_XY.x;
        //currentMax_XY.x = blockManager.bounds.max.x - blockData.max_XY.x;
        //currentMin_XY.y = blockManager.bounds.max.y - blockData.max_XY.y;
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
        currentMax_XY.y = blockManager.bounds.max.y - blockData.max_XY.y;
        currentMin_XY.y = blockManager.bounds.min.y - blockData.min_XY.y; 

        // some formula to equate position to gric rows and columns.. grid column 0 = -11.5 x   grid row 0 = -19.5y
        pivotPointGridColumn = Mathf.RoundToInt(transform.position.x + 11.5f);
        pivotPointGridRow = Mathf.RoundToInt(transform.position.y + 19.5f);

        //for (int i = 0; i < 5; i++)
        //{
        //    Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red);
        //}

        if (transform.position.y <= currentMax_XY.y)
        {
            for (int i = 0; i < 5; i++)
            {
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.green);
                occupiedGridCells[i].x = Mathf.RoundToInt(subBlocks[i].transform.position.x + 11.5f);
                occupiedGridCells[i].y = Mathf.RoundToInt(subBlocks[i].transform.position.y + 19.5f);
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red); // null error here, because of block moving beyond boundary
                Debug.Log("falling block occupied grid cells = " + (int)occupiedGridCells[i].x + ", " + (int)occupiedGridCells[i].y);
            }
        }

        Debug.Log(name + " currentMin = " + currentMin_XY + " , currentMax = " + currentMax_XY);
    }

    public void CollisionCheck()
    {

        occupiedAdjacentCells.Clear();
        for (int i = 0; i < 3; i++)
        {
            for (int i2 = 0; i2 < 3; i2++)
            {
                if (Grid.cells[i,i2].isFilled)
                {
                    bool occupiedBySelf = false;
                    for(int i3 = 0; i3 < 5; i3++)
                    {
                        if (occupiedGridCells[i3].x == i && occupiedGridCells[i3].y == i2)
                        {
                            occupiedBySelf = true;
                            i3 = 6;
                        }
                        if (!occupiedBySelf)
                        {
                            // Add cell grid coordinates to list
                            occupiedAdjacentCells.Add(new Vector2(i, i2));
                        }
                    }
                }
            }
        }

        // Check if added cells are blocking movement or rotation before next change.
    }

    public void CheckBlockAgainstBounds()
    {
        // If position.x is greater than bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less), there is still room to move left
        if (transform.position.x >= currentMin_XY.x)//blockManager.bounds.min.x - currentMin_XY.x)
            canMoveLeft = true;
        else
        {
            canMoveLeft = false;
        }

        // If fallingBlock position.x is less than bounds.max.x (11.5) minus fallingBlock's max.x (always 0 or more), there is still room to move right
        if (transform.position.x <= currentMax_XY.x )// blockManager.bounds.max.x - currentMax_XY.x)
            canMoveRight = true;
        else
            canMoveRight = false;

    }

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        for (int i = 0; i < 5; i++)
        {
            if (!blockManager.bounds.Contains(subBlocks[i].transform.position))
            {
                transform.Rotate(new Vector3(0, 0, -90));
                i = 6;
            }
        }
        UpdatePositionData();
    }

    public void Fall()
    {
        transform.position += Vector3.down;
        UpdatePositionData();
    }

    //public void MoveBackInsideBounds()
    //{
    //    //C-2 Movement - Snap back inside bounds
    //    Vector2 tempPosition = transform.position;
    //    float tempX = transform.position.x;
    //    float tempY = transform.position.y;
    //
    //    for (int i = 0; i < 5; i++)
    //    {
    //        if (!blockManager.bounds.Contains(subBlocks[i].transform.position))
    //        {
    //            //New
    //            tempX = subBlocks[i].transform.position.x;
    //            //End of new
    //            if (tempX < blockManager.bounds.min.x)
    //            {
    //                blockManager.ResetKeys();
    //
    //                tempPosition.x = Mathf.Round(tempPosition.x);
    //                if (tempPosition.x < blockManager.bounds.min.x)
    //                {
    //                    tempPosition.x++;
    //                }
    //                transform.position = tempPosition;
    //            }
    //            else if (tempX > blockManager.bounds.max.x)
    //            {
    //                blockManager.ResetKeys();
    //
    //                tempPosition.x = Mathf.Round(tempPosition.x);
    //                if (tempPosition.x > blockManager.bounds.min.x)
    //                {
    //                    tempPosition.x--;
    //                }
    //                transform.position = tempPosition;
    //            }
    //            i = 6;
    //        }
    //    }
    //}

    public void InputMovement(bool horizontal, float value)
    {
        if (horizontal)
        {
            transform.position += Vector3.right * value;
            CheckBlockAgainstBounds();
            //MoveBackInsideBounds();
        }
        else
        {
            transform.position += Vector3.up * value;
            CheckBlockAgainstBounds();
            //MoveBackInsideBounds();
        }
    }

    // Called on key up
    public void SnapToGrid()
    {
        //Get Position;
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        //Get offset from nearest 0.5
        float xOffset = xPosition > 0 ? (xPosition % 0.5f) : (xPosition % -0.5f);
        //Get offset from nearest 0.5
        float yOffset = yPosition > 0 ? (yPosition % 0.5f) : (yPosition % -0.5f);

        Debug.Log("SnapToGrid called, y position is " + yPosition);

        if (Mathf.Abs(yPosition - Mathf.Round(yPosition)) < 0.02f)
        {
            yPosition = Mathf.Round(yPosition);

            Debug.Log("yPosition apx int, yPosition rounded to int");
        }


        // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
        if (!blockManager.DivisibleByHalfAndNotOne(transform.position.x))
        {


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
            Debug.Log("1.Snapping y position");

            Debug.Log("2.Y Offset = " + yOffset);
            //Round down if offset less than 0.25
            if (Mathf.Abs(yOffset) < 0.25f)
            {
                Debug.Log("yOffset < 0.25");
                // Operation will always reduce absolute value, so should never result in block moving out of bounds unless it already was.
                yPosition -= yOffset;
                if (!blockManager.DivisibleByHalfAndNotOne(yPosition))
                {
                    yPosition += yPosition < 0 ? 0.5f : -0.5f;
                }
            }
            //Round up if offset greater than 0.25
            else if (Mathf.Abs(yOffset) > 0.25f)
            {
                Debug.Log("yOffset > 0.25");
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
            Debug.Log("3. New Y Position = " + yPosition);
        }
        //Apply corrected position
        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
    }


    public void NewSnapToGrid()
    {
        float x = transform.position.x;
        float y = transform.position.y;


        // If x % 1 is leass than 0.05, number is basically and int, so just round it and +/- 0.5
        if (x % 1 < 0.05)
        {

        }
        // Otherwise, 

        //Get offset from nearest 0.5
        float xOffset = x > 0 ? (x % 0.5f) : (x % -0.5f);
        //Get offset from nearest 0.5
        float yOffset = y > 0 ? (y % 0.5f) : (y % -0.5f);


        // If position.x is not divisible by only 0.5
        if (!blockManager.DivisibleByHalfAndNotOne(x))
        {
            // Declare float newX. newX is equal to this block's position.x rounded to the nearest integer  //TODO: Rename newX to integerX;
            float newX = Mathf.Round(x);
            //If difference between position.x and rounded x is significant
            if (Mathf.Abs(x - newX) >= 0.03f)
            {
                // Negative x values
                if (newX < 0)
                {
                    //If (newX - 0.5) is greater than or equal to this block's lowest allowable x position, value will be within boundaries, so,        SUBTRACT 0.5 from newX
                    if (newX - 0.5f >= currentMin_XY.x)
                    {
                        newX -= 0.5f;
                    }
                    //If (newX - 0.5) is less than this block's lowest allowable x position, value will be outside of boundaries, so,                   ADD 0.5 to newX instead
                    else
                        newX += 0.5f;
                }
                // Positive x values
                else if (newX > 0)
                {
                    //If (newX + 0.5) is less than or equal to highest allowable x position, value will be within boundaries, so,                       ADD 0.5 to newX
                    if (newX + 0.5f <= currentMax_XY.x)
                    {
                        newX += 0.5f;
                    }
                    //If (newX + 0.5) is greater than this block's highest allowable x position, value will be outside of boundaries, so,               SUBTRACT 0.5 from newX
                    else
                        newX -= 0.5f;
                }
            }
            //else

            x = newX;
        }
        if (!blockManager.DivisibleByHalfAndNotOne(y))
        {
            float newY = Mathf.Round(y);
            if (Mathf.Abs(y - newY) >= 0.03f)
            {
                if (newY < 0)
                {
                    if (newY - 0.5f >= currentMin_XY.y)
                    {
                        newY -= 0.5f;
                    }
                    else
                        newY += 0.5f;
                }
                else if (newY > 0)
                {
                    if (newY + 0.5f <= currentMax_XY.y)
                    {
                        newY += 0.5f;
                    }
                    else
                        newY -= 0.5f;
                }
            }
            y = newY;
        }
        transform.position = new Vector3(x, y, 0);
    }
}
