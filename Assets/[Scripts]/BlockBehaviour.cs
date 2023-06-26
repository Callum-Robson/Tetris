using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public enum MoveType
{
    FALL,
    INPUT,
    ROTATION
}
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
    public SubBlockBehaviour[] subBlocks = new SubBlockBehaviour[5];


    public List<int> rowsOccupied = new List<int>();
    public List<int> columnsOccupied = new List<int>();

    public bool stopped = false;
    public bool canMoveLeft, canMoveRight, canMoveDown;

    private void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        //subBlocks = GetComponentsInChildren<SubBlockBehaviour>();
        //PrefabUtility.ApplyPrefabInstance(this.gameObject, InteractionMode.AutomatedAction);
        currentMin_XY = blockData.min_XY;
        currentMax_XY = blockData.max_XY;
        currentSubBlockCoordinates = blockData.subBlockCoordinates;

        UpdatePositionData();
    }

    // Update is called once per frame
    void Update()
    {
       if (transform.position.y <= currentMin_XY.y && !stopped)
       {
           stopped = true;
           blockManager.ResetKeys();
       }
    }

    public void CollisionCheck(MoveType moveType, bool horizontal, float value)
    {
        Debug.Log("CollisionCheck called");
        // 1-3 Calculate block boundaries, and grid space position of pivot point
        #region PositionAndBoundaries
        //  1. Get current subBlock positions
        for (int i = 0; i < 5; i++)
        {
            currentSubBlockCoordinates[i] = subBlocks[i].transform.position;
        }

        //  2. Calculate current boundaries of entire block                     Bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less) will always be greater than Bounds.min.x
        currentMin_XY.x = (int)Mathf.Round((blockManager.bounds.min.x - blockData.min_XY.x));
        currentMax_XY.x = (int)Mathf.Round((blockManager.bounds.max.x - blockData.max_XY.x));
        currentMax_XY.y = (int)Mathf.Round((blockManager.bounds.max.y - blockData.max_XY.y));
        currentMin_XY.y = (int)Mathf.Round((blockManager.bounds.min.y - blockData.min_XY.y));

        //  3. Calculate grid-space coordinates of block's pivot point
        pivotPointGridColumn = (int)Mathf.Round(transform.position.x);
        pivotPointGridRow = (int)Mathf.Round(transform.position.y);
        #endregion

       ////  4. Update grid cells filled to true or false
       //#region UpdateGridCellsFilledState
       //Debug.Log("Grid length x = " + Grid.cells.GetLength(0));
       //Debug.Log("Grid length y = " + Grid.cells.GetLength(1));
       //for (int i = 0; i < occupiedGridCells.Length; i++)
       //{
       //    if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
       //    {
       //        Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(false);
       //        //Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.green);
       //    }
       //    else
       //    {
       //        Debug.Log("Couldn't reset grid cell, an index was out of bounds");
       //        Debug.Log("Actual position.x was = " + subBlocks[i].transform.position.x);
       //        Debug.Log("Index x was = " + (occupiedGridCells[i].x));
       //        Debug.Log("Actual position.y was = " + subBlocks[i].transform.position.y);
       //        Debug.Log("Index y was = " + (occupiedGridCells[i].y));
       //    }
       //    occupiedGridCells[i].x = Mathf.Round(subBlocks[i].transform.position.x);
       //    occupiedGridCells[i].y = Mathf.Round(subBlocks[i].transform.position.y);
       //    if (occupiedGridCells[i].x >= Grid.cells.GetLength(0) - 1)
       //    {
       //        Debug.Log("Index x: " + occupiedGridCells[i].x + "is invalid");
       //    }
       //    if (occupiedGridCells[i].y >= Grid.cells.GetLength(1) - 1)
       //    {
       //        Debug.Log("Index y: " + occupiedGridCells[i].y + "is invalid");
       //    }
       //
       //    if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
       //    {
       //        Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(true);
       //        //Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red); // null error here, because of block moving beyond boundary
       //        Debug.Log("falling block occupied grid cell #" + i + " = " + (int)occupiedGridCells[i].x + ", " + (int)occupiedGridCells[i].y);
       //    }
       //
       //}
       //#endregion

        //  5. Calculate what cells to check for collision with this block
        #region FindPotentialCollisions
        int maxTopCellToCheck, maxBottomCellToCheck;
        int maxRightCellToCheck, maxLeftCellToCheck;

        maxTopCellToCheck = 39 - pivotPointGridRow < 3 ? 39 - pivotPointGridRow : 3;
        maxBottomCellToCheck = pivotPointGridRow < 3 ? -pivotPointGridRow : -3;
        maxRightCellToCheck = pivotPointGridColumn > 20 ? 23 - pivotPointGridColumn : 3;
        maxLeftCellToCheck = pivotPointGridColumn < 3 ? -pivotPointGridColumn : -3;

        int actualTopCellToCheck = pivotPointGridRow + maxTopCellToCheck;           //pivotPointGridRow + 3 < maxTopCellToCheck ? pivotPointGridRow + 3 :pivotPointGridRow + maxTopCellToCheck;
        int actualBottomCellToCheck = pivotPointGridRow + maxBottomCellToCheck;     //pivotPointGridRow - 3 > maxBottomCellToCheck ? pivotPointGridRow - 3 : pivotPointGridRow + maxBottomCellToCheck;
        int actualRightCellToCheck = pivotPointGridColumn + maxRightCellToCheck;    //pivotPointGridColumn + 3 < maxRightCellToCheck ? pivotPointGridColumn + 3 : pivotPointGridColumn + maxRightCellToCheck;
        int actualLeftCellToCheck = pivotPointGridColumn + maxLeftCellToCheck;      //pivotPointGridColumn - 3 > maxLeftCellToCheck ? pivotPointGridColumn - 3 : pivotPointGridColumn + maxLeftCellToCheck;

        List<Vector2> occupiedAdjacentCells = new List<Vector2>();
        List<Vector2> filledCells = new List<Vector2>();

        //Iterate through columns from maxLeft to maxRight
        for (int i = actualLeftCellToCheck; i <= actualRightCellToCheck; i++)
        {

            //Iterate through rows from maxBottom to maxTop
            for (int i2 = actualBottomCellToCheck; i2 <= actualTopCellToCheck; i2++)
            {
                if (Grid.cells[i, i2].GetFilledState())
                {
                    filledCells.Add(new Vector2(i, i2));
                }
            }
        }

        occupiedAdjacentCells.Clear();

        for ( int i = 0; i < filledCells.Count; i++)
        {
            bool matchedCell = false;
            for (int i2 = 0; i2 < 5; i2++)
            {
                if (filledCells[i].x == occupiedGridCells[i2].x && filledCells[i].y == occupiedGridCells[i2].y)
                {
                    matchedCell = true;
                }
            }
            if (!matchedCell)
                occupiedAdjacentCells.Add(filledCells[i]);
        }

        #endregion

        //  6. Check if any filled cells will stop block from moving in it's current direction
        #region ActuallyCheckCollision
        for (int i = 0; i < occupiedAdjacentCells.Count; i++)
        {
            for (int i2 = 0; i2 < 5; i2++)
            {
                if (occupiedAdjacentCells[i].x == occupiedGridCells[i2].x && occupiedAdjacentCells[i].y == occupiedGridCells[i2].y - 1)
                {
                    stopped = true;
                }
            }
        }
        #endregion

        #region MaybeOldCollisionCheck
        ////Iterate through columns from maxLeft to maxRight
        //for (int i = actualLeftCellToCheck; i <= actualRightCellToCheck;  i++)
        //{
        //
        //    //Iterate through rows from maxBottom to maxTop
        //    for (int i2 = actualBottomCellToCheck; i2 <= actualTopCellToCheck; i2++)
        //    {
        //        if (Grid.cells[i,i2].isFilled)
        //        {
        //            filledCells.Add(Grid.cells[i, i2].position);
        //            bool occupiedBySelf = false;
        //
        //            for (int a = 0; a < 5; a++)
        //            {
        //                if (occupiedGridCells[a].x != i && occupiedGridCells[a].y != i2)
        //                {
        //                    occupiedBySelf = false;
        //                }
        //            }
        //
        //            for (int i3 = 0; i3 < 5; i3++)
        //            {
        //                if (occupiedGridCells[i3].x == i && occupiedGridCells[i3].y == i2)
        //                {
        //                    occupiedBySelf = true;
        //                    i3 = 6;
        //                }
        //                if (!occupiedBySelf)
        //                {
        //                    // Add cell grid coordinates to list
        //                    occupiedAdjacentCells.Add(new Vector2(i, i2));
        //                    // This probably doesnt in any way check specifically the bottom subblocks of the block aginst the ones under it...
        //                    if (occupiedAdjacentCells[occupiedAdjacentCells.Count - 1].y == occupiedGridCells[i3].y - 1)
        //                    {
        //                        Debug.Log("Block landed on another block");
        //                        stopped = true;
        //                        occupiedAdjacentCells.Clear();
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        // Check if added cells are blocking movement or rotation before next change.

        #endregion

        blockManager.StateMachine.SetState(TheStateMachine.GameplayState.UpdatingActiveBlock);

        switch (moveType)
        {
            case (MoveType.FALL):
                Fall();
                break;
            case (MoveType.INPUT):
                InputMovement(horizontal, value);
                break;
            case (MoveType.ROTATION):
                Rotate();
                break;

        }
    }



    public void UpdatePositionData()
    {
       // for (int i = 0; i < 5; i++)
       // {
       //     currentSubBlockCoordinates[i] = subBlocks[i].transform.position;
       // }
       //
       // // Bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less) will always be greater than Bounds.min.x
       // currentMin_XY.x = blockManager.bounds.min.x - blockData.min_XY.x;
       // currentMax_XY.x = blockManager.bounds.max.x - blockData.max_XY.x;
       // currentMax_XY.y = blockManager.bounds.max.y - blockData.max_XY.y;
       // currentMin_XY.y = blockManager.bounds.min.y - blockData.min_XY.y; 
       //
       // // some formula to equate position to gric rows and columns.. grid column 0 = -11.5 x   grid row 0 = -19.5y
       // pivotPointGridColumn = Mathf.RoundToInt(transform.position.x + 11.5f);
       // pivotPointGridRow = Mathf.RoundToInt(transform.position.y + 19.5f);
       //
       //
       // if (transform.position.y <= currentMax_XY.y)
       // {
       //     for (int i = 0; i < occupiedGridCells.Length; i++)
       //     {
       //         
       //         Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].isFilled = false;
       //         Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.green);
       //         occupiedGridCells[i].x = Mathf.RoundToInt(subBlocks[i].transform.position.x + 11.5f);
       //         occupiedGridCells[i].y = Mathf.RoundToInt(subBlocks[i].transform.position.y + 19.5f);
       //         Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].isFilled = true;
       //         Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red); // null error here, because of block moving beyond boundary
       //         Debug.Log("falling block occupied grid cells = " + (int)occupiedGridCells[i].x + ", " + (int)occupiedGridCells[i].y);
       //     }
       // }
       //
       // Debug.Log(name + " currentMin = " + currentMin_XY + " , currentMax = " + currentMax_XY);
    }

    private void ResetDebugGrid(Action callback)
    {
        for (int i = 0; i < occupiedGridCells.Length; i++)
        {
            if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
            {
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(false);
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.green);
            }
            else
            {
                Debug.Log("Couldn't reset grid cell, an index was out of bounds");
                Debug.Log("Actual position.x was = " + subBlocks[i].transform.position.x);
                Debug.Log("Index x was = " + (occupiedGridCells[i].x));
                Debug.Log("Actual position.y was = " + subBlocks[i].transform.position.y);
                Debug.Log("Index y was = " + (occupiedGridCells[i].y));
            }
        }
        callback?.Invoke();
    }

    private void UpdateDebugGrid(Action callback)
    {
        for (int i = 0; i < occupiedGridCells.Length; i++)
        {
            occupiedGridCells[i].x = subBlocks[i].transform.position.x;
            occupiedGridCells[i].y = subBlocks[i].transform.position.y;
            if (occupiedGridCells[i].x >= Grid.cells.GetLength(0) - 1)
            {
                Debug.Log("Index x: " + occupiedGridCells[i].x + "is invalid");
            }
            if (occupiedGridCells[i].y >= Grid.cells.GetLength(1) - 1)
            {
                Debug.Log("Index y: " + occupiedGridCells[i].y + "is invalid");
            }

            if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
            {
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(true);
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red); // null error here, because of block moving beyond boundary
                Debug.Log("falling block occupied grid cells = " + (int)occupiedGridCells[i].x + ", " + (int)occupiedGridCells[i].y);
            }

        }
        callback?.Invoke();
    }

   

    public void CheckBlockAgainstBounds()
    {
        // If position.x is greater than bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less), there is still room to move left
        if (transform.position.x > currentMin_XY.x)//blockManager.bounds.min.x - currentMin_XY.x)
            canMoveLeft = true;
        else
        {
            canMoveLeft = false;
        }

        // If fallingBlock position.x is less than bounds.max.x (11.5) minus fallingBlock's max.x (always 0 or more), there is still room to move right
        if (transform.position.x < currentMax_XY.x )// blockManager.bounds.max.x - currentMax_XY.x)
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
        blockManager.StateMachine.SetState(TheStateMachine.GameplayState.UpdatingOtherBlocks);
    }

    public void Fall()
    {
        Debug.Log("Fall called");
        if (!stopped)
        {
            transform.position += Vector3.down;
        }
        UpdatePositionData();

        // 1-3 Calculate block boundaries, and grid space position of pivot point
        //  1. Get current subBlock positions
        for (int i = 0; i < 5; i++)
        {
            currentSubBlockCoordinates[i] = subBlocks[i].transform.position;
        }

        //  2. Calculate current boundaries of entire block                     Bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less) will always be greater than Bounds.min.x
        currentMin_XY.x = (int)Mathf.Round((blockManager.bounds.min.x - blockData.min_XY.x));
        currentMax_XY.x = (int)Mathf.Round((blockManager.bounds.max.x - blockData.max_XY.x));
        currentMax_XY.y = (int)Mathf.Round((blockManager.bounds.max.y - blockData.max_XY.y));
        currentMin_XY.y = (int)Mathf.Round((blockManager.bounds.min.y - blockData.min_XY.y));

        //  3. Calculate grid-space coordinates of block's pivot point
        pivotPointGridColumn = (int)Mathf.Round(transform.position.x);
        pivotPointGridRow = (int)Mathf.Round(transform.position.y);

        //  4. Update grid cells filled to true or false
        Debug.Log("Grid length x = " + Grid.cells.GetLength(0));
        Debug.Log("Grid length y = " + Grid.cells.GetLength(1));
        for (int i = 0; i < occupiedGridCells.Length; i++)
        {
            if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
            {
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(false);
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.green);
            }
        }

        for (int i = 0; i < occupiedGridCells.Length; i++)
        {
            occupiedGridCells[i].x = Mathf.Round(subBlocks[i].transform.position.x);
            occupiedGridCells[i].y = Mathf.Round(subBlocks[i].transform.position.y);
            if (occupiedGridCells[i].x >= Grid.cells.GetLength(0))
            {
                Debug.Log("Index x: " + occupiedGridCells[i].x + "is invalid");
            }
            if (occupiedGridCells[i].y >= Grid.cells.GetLength(1))
            {
                Debug.Log("Index y: " + occupiedGridCells[i].y + "is invalid");
            }

            if (occupiedGridCells[i].x < Grid.cells.GetLength(0) && occupiedGridCells[i].y < Grid.cells.GetLength(1))
            {
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetFilledState(true);
                Grid.cells[(int)occupiedGridCells[i].x, (int)occupiedGridCells[i].y].SetColor(Color.red); // null error here, because of block moving beyond boundary
                Debug.Log("falling block occupied grid cell #" + i + " = " + (int)occupiedGridCells[i].x + ", " + (int)occupiedGridCells[i].y);
            }

        }

        blockManager.StateMachine.SetState(TheStateMachine.GameplayState.UpdatingOtherBlocks);
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
            //CheckBlockAgainstBounds();
            if (!stopped)
            {
                transform.position += Vector3.right * value;

                //NEW Start
                UpdatePositionData();
                //NEW End
            }

            //CheckBlockAgainstBounds();
            //MoveBackInsideBounds();

        }
        else
        {
            //CheckBlockAgainstBounds();

            if(!stopped)
            {
                transform.position += Vector3.up * value;

                //NEW Start
                UpdatePositionData();
                //NEW End
            }

            //CheckBlockAgainstBounds();

            //MoveBackInsideBounds();
        }
        blockManager.StateMachine.SetState(TheStateMachine.GameplayState.UpdatingOtherBlocks);
    }

 
}

// No longer needed, only moving in exact increments

//// Called on key up
//public void SnapToGrid()
//{
//    //Get Position;
//    float xPosition = transform.position.x;
//    float yPosition = transform.position.y;
//    //Get offset from nearest 0.5
//    float xOffset = xPosition > 0 ? (xPosition % 0.5f) : (xPosition % -0.5f);
//    //Get offset from nearest 0.5
//    float yOffset = yPosition > 0 ? (yPosition % 0.5f) : (yPosition % -0.5f);
//
//    Debug.Log("SnapToGrid called, y position is " + yPosition);
//
//    if (Mathf.Abs(yPosition - Mathf.Round(yPosition)) < 0.02f)
//    {
//        yPosition = Mathf.Round(yPosition);
//
//        Debug.Log("yPosition apx int, yPosition rounded to int");
//    }
//
//
//    // If x position is divisible by 0.5 and not divisible by 1... it wouldn't need to be snapped, so check if that's not true, and width is also not even (divisble by 2)
//    if (!blockManager.DivisibleByHalfAndNotOne(transform.position.x))
//    {
//
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
//            if (xPosition > 0 && xPosition + roundingValue <= currentMax_XY.x)
//            {
//                xPosition += roundingValue;
//            }
//            // If xPosition is negative and xPosition rounded "up" is greater than min position, make it so
//            else if (xPosition < 0 && xPosition + roundingValue >= currentMin_XY.x)
//            {
//                xPosition += roundingValue;
//            }
//        }
//    }
//
//    if (!blockManager.DivisibleByHalfAndNotOne(transform.position.y))
//    {
//        Debug.Log("1.Snapping y position");
//
//        Debug.Log("2.Y Offset = " + yOffset);
//        //Round down if offset less than 0.25
//        if (Mathf.Abs(yOffset) < 0.25f)
//        {
//            Debug.Log("yOffset < 0.25");
//            // Operation will always reduce absolute value, so should never result in block moving out of bounds unless it already was.
//            yPosition -= yOffset;
//            if (!blockManager.DivisibleByHalfAndNotOne(yPosition))
//            {
//                yPosition += yPosition < 0 ? 0.5f : -0.5f;
//            }
//        }
//        //Round up if offset greater than 0.25
//        else if (Mathf.Abs(yOffset) > 0.25f)
//        {
//            Debug.Log("yOffset > 0.25");
//            // Round down absolute value
//            yPosition -= yOffset;
//            // If rounding "up" for negative number, subtract 0.5, if rounding up for position, add 0.5
//            float roundingValue = yOffset < 0 ? -0.5f : 0.5f;
//            // If yPosition is positive and yPosition rounded up is less than max position, make it so
//            if (yPosition > 0 && yPosition + roundingValue <= currentMax_XY.y)
//            {
//                yPosition += roundingValue;
//            }
//            // If yPosition is negative and yPosition rounded "up" is greater than min position, make it so
//            else if (yPosition < 0 && yPosition + roundingValue >= currentMin_XY.y)
//            {
//                yPosition += roundingValue;
//            }
//        }
//        Debug.Log("3. New Y Position = " + yPosition);
//    }
//    //Apply corrected position
//    transform.position = new Vector3(xPosition, yPosition, transform.position.z);
//}
//
//
//public void NewSnapToGrid()
//{
//    float x = transform.position.x;
//    float y = transform.position.y;
//
//
//    // If x % 1 is leass than 0.05, number is basically and int, so just round it and +/- 0.5
//    if (x % 1 < 0.05)
//    {
//
//    }
//    // Otherwise, 
//
//    //Get offset from nearest 0.5
//    float xOffset = x > 0 ? (x % 0.5f) : (x % -0.5f);
//    //Get offset from nearest 0.5
//    float yOffset = y > 0 ? (y % 0.5f) : (y % -0.5f);
//
//
//    // If position.x is not divisible by only 0.5
//    if (!blockManager.DivisibleByHalfAndNotOne(x))
//    {
//        // Declare float newX. newX is equal to this block's position.x rounded to the nearest integer  //TODO: Rename newX to integerX;
//        float newX = Mathf.Round(x);
//        //If difference between position.x and rounded x is significant
//        if (Mathf.Abs(x - newX) >= 0.03f)
//        {
//            // Negative x values
//            if (newX < 0)
//            {
//                //If (newX - 0.5) is greater than or equal to this block's lowest allowable x position, value will be within boundaries, so,        SUBTRACT 0.5 from newX
//                if (newX - 0.5f >= currentMin_XY.x)
//                {
//                    newX -= 0.5f;
//                }
//                //If (newX - 0.5) is less than this block's lowest allowable x position, value will be outside of boundaries, so,                   ADD 0.5 to newX instead
//                else
//                    newX += 0.5f;
//            }
//            // Positive x values
//            else if (newX > 0)
//            {
//                //If (newX + 0.5) is less than or equal to highest allowable x position, value will be within boundaries, so,                       ADD 0.5 to newX
//                if (newX + 0.5f <= currentMax_XY.x)
//                {
//                    newX += 0.5f;
//                }
//                //If (newX + 0.5) is greater than this block's highest allowable x position, value will be outside of boundaries, so,               SUBTRACT 0.5 from newX
//                else
//                    newX -= 0.5f;
//            }
//        }
//        //else
//
//        x = newX;
//    }
//    if (!blockManager.DivisibleByHalfAndNotOne(y))
//    {
//        float newY = Mathf.Round(y);
//        if (Mathf.Abs(y - newY) >= 0.03f)
//        {
//            if (newY < 0)
//            {
//                if (newY - 0.5f >= currentMin_XY.y)
//                {
//                    newY -= 0.5f;
//                }
//                else
//                    newY += 0.5f;
//            }
//            else if (newY > 0)
//            {
//                if (newY + 0.5f <= currentMax_XY.y)
//                {
//                    newY += 0.5f;
//                }
//                else
//                    newY -= 0.5f;
//            }
//        }
//        y = newY;
//    }
//    transform.position = new Vector3(x, y, 0);
//}