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
    /* 
     public BlockData blockData;
     private Vector2 currentMin_XY;
     private Vector2 currentMax_XY;
     private Vector2[] currentSubBlockCoordinates = new Vector2[5];

     public Vector2[] CurrentSubBlockCoordinates { get { return currentSubBlockCoordinates; } }

     private int pivotPointGridColumn;
     private int pivotPointGridRow;
     private Vector2[] occupiedGridCells = new Vector2[5];

     public BlockManager blockManager;
     public SubBlockBehaviour[] subBlocks = new SubBlockBehaviour[5];



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
        else
             CheckBlockAgainstBounds();
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
         currentMin_XY.x = Mathf.RoundToInt((blockManager.bounds.min.x - blockData.min_XY.x));
         currentMax_XY.x = Mathf.RoundToInt((blockManager.bounds.max.x - blockData.max_XY.x));
         currentMax_XY.y = Mathf.RoundToInt((blockManager.bounds.max.y - blockData.max_XY.y));
         currentMin_XY.y = Mathf.RoundToInt((blockManager.bounds.min.y - blockData.min_XY.y));

         //  3. Calculate grid-space coordinates of block's pivot point
         pivotPointGridColumn = Mathf.RoundToInt(transform.position.x);
         pivotPointGridRow = Mathf.RoundToInt(transform.position.y);
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
         currentMin_XY.x = Mathf.RoundToInt((blockManager.bounds.min.x - blockData.min_XY.x));
         currentMax_XY.x = Mathf.RoundToInt((blockManager.bounds.max.x - blockData.max_XY.x));
         currentMax_XY.y = Mathf.RoundToInt((blockManager.bounds.max.y - blockData.max_XY.y));
         currentMin_XY.y = Mathf.RoundToInt((blockManager.bounds.min.y - blockData.min_XY.y));

         //  3. Calculate grid-space coordinates of block's pivot point
         pivotPointGridColumn = Mathf.RoundToInt(transform.position.x);
         pivotPointGridRow = Mathf.RoundToInt(transform.position.y);

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
             occupiedGridCells[i].x = Mathf.RoundToInt(subBlocks[i].transform.position.x);
             occupiedGridCells[i].y = Mathf.RoundToInt(subBlocks[i].transform.position.y);
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
    */


    public BlockManager blockManager;
    public BlockData blockData;
    public SubBlockBehaviour[] subBlocks = new SubBlockBehaviour[5];

    private int gridX, gridY = 0;

    private Vector2 currentMin_XY;
    private Vector2 currentMax_XY;

    public bool stopped = false;
    public bool canMoveLeft, canMoveRight, canMoveDown;

    private int cellsToCheckAbove, cellsToCheckBelow;
    private int cellsToCheckRight, cellsToCheckLeft;

    private int maxTopCell = 0;
    private int maxBottomCell = 0;
    private int maxRightCell = 0;
    private int maxLeftCell = 0;

    // The distance in cells from the pivot point
    private Vector2 defaultMin, defaultMax;
    private Vector2 activeMin, activeMax;
    private bool isRotated = false;
    private int degreesRotated = 0;

    private bool cellsReset = false;

    private void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        //subBlocks = GetComponentsInChildren<SubBlockBehaviour>();
        //PrefabUtility.ApplyPrefabInstance(this.gameObject, InteractionMode.AutomatedAction);
        currentMin_XY = blockData.min_XY;
        currentMax_XY = blockData.max_XY;

        //UpdatePositionData();
        defaultMin = blockData.min_XY;
        defaultMax = blockData.max_XY;
        // Redundant?
        activeMin = defaultMin;
        activeMax = defaultMax;

    }


    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            canMoveDown = true;
        }
        if (transform.position.y <= currentMin_XY.y && !stopped)
        {
            stopped = true;
            blockManager.ResetKeys();
        }
        // else
        //   CheckBlockAgainstBounds();
    }

    public void UpdateCanMove()
    {
        // If position.x is greater than bounds.min.x (-11.5) minus fallingBlock's min.x (always 0 or less), there is still room to move left
        if (transform.position.x > currentMin_XY.x)//blockManager.bounds.min.x - currentMin_XY.x)
            canMoveLeft = true;
        else
        {
            canMoveLeft = false;
        }

        // If fallingBlock position.x is less than bounds.max.x (11.5) minus fallingBlock's max.x (always 0 or more), there is still room to move right
        if (transform.position.x < currentMax_XY.x)// blockManager.bounds.max.x - currentMax_XY.x)
            canMoveRight = true;
        else
            canMoveRight = false;

    }

    public void CollisionCheck(MoveType moveType, bool horizontal, float value)
    {
        Debug.Log("CollisionCheck called");

        #region PositionAndBoundaries

        // Set Grid Coordinates
        gridX = (int)Mathf.Round(transform.position.x);
        gridY = (int)Mathf.Round(transform.position.y);
        // Calculate current boundaries of entire block (right now this only works for unrotated pentominos)

        currentMin_XY.x = (int)Mathf.Round((blockManager.bounds.min.x - activeMin.x));
        currentMax_XY.y = (int)Mathf.Round((blockManager.bounds.max.y - activeMax.y));
        currentMin_XY.y = (int)Mathf.Round((blockManager.bounds.min.y - activeMin.y));
        currentMax_XY.x = (int)Mathf.Round((blockManager.bounds.max.x - activeMax.x));

        #endregion

        //  Calculate what cells to check for collision with this block
        #region FindPotentialCollisions

        cellsToCheckAbove = 39 - gridY < 3 ? 39 - gridY : 3;
        cellsToCheckBelow = gridY < 3 ? -gridY : -3;
        cellsToCheckRight = gridX > 20 ? 23 - gridX : 3;
        cellsToCheckLeft = gridX < 3 ? -gridX : -3;

        maxTopCell = gridY + cellsToCheckAbove;
        maxBottomCell = gridY + cellsToCheckBelow;
        maxRightCell = gridX + cellsToCheckRight;
        maxLeftCell = gridX + cellsToCheckLeft;

        List<Vector2> occupiedAdjacentCells = new List<Vector2>();
        List<Vector2> filledCells = new List<Vector2>();

        //Iterate through columns from maxLeft to maxRight
        for (int i = maxLeftCell; i <= maxRightCell; i++)
        {
            //Iterate through rows from maxBottom to maxTop
            for (int i2 = maxBottomCell; i2 <= maxTopCell; i2++)
            {
                if (Grid.cells[i, i2].GetFilledState())
                {
                    filledCells.Add(new Vector2(i, i2));
                }
            }
        }

        occupiedAdjacentCells.Clear();

        for (int i = 0; i < filledCells.Count; i++)
        {
            bool matchedCell = false;
            for (int i2 = 0; i2 < 5; i2++)
            {
                if (filledCells[i].x == subBlocks[i2].transform.position.x && filledCells[i].y == subBlocks[i2].transform.position.y)
                {
                    matchedCell = true;
                }
            }
            if (!matchedCell)
                occupiedAdjacentCells.Add(filledCells[i]);
        }

        #endregion

        //  5. Check if any filled cells are directly below any subblock
        #region ActuallyCheckCollision
        for (int i = 0; i < occupiedAdjacentCells.Count; i++)
        {
            for (int i2 = 0; i2 < 5; i2++)
            {
                if (occupiedAdjacentCells[i].x == subBlocks[i2].transform.position.x && occupiedAdjacentCells[i].y == subBlocks[i2].transform.position.y - 1)
                {
                    stopped = true;
                }
            }
        }
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
                Rotate(true);
                break;

        }
    }

    public void Fall()
    {
        Debug.Log("PentominoBehaviour - Fall");
        if (cellsReset)
        {
            if (!stopped)
            {
                transform.position += Vector3.down;
                UpdateDebugGrid();
            }
        }
        else
        {
            ResetDebugGrid(MoveType.FALL, false, 0);
        }
    }

    public void InputMovement(bool horizontal, float value)
    {
        Debug.Log("PentominoBehaviour - InputMovement");
        Debug.Log("Horizontal = " + horizontal + " and value = " + value);
        if (cellsReset)
        {
            // movement
            if (horizontal)
            {
                if (!stopped)
                {
                    transform.position += Vector3.right * value;

                }
            }
            else
            {
                if (!stopped)
                {
                    transform.position += Vector3.up * value;
                }
            }
            UpdateDebugGrid();
        }
        else
        {
            ResetDebugGrid(MoveType.INPUT, horizontal, value);
        }
    }

    public void Rotate(bool clockwise)
    {
        if (cellsReset)
        {
            degreesRotated += 90;
            if (degreesRotated == 360)
                degreesRotated = 0;
            Debug.Log("PentominoBehaviour - Rotate");
            if (clockwise)
            {
                switch (degreesRotated)
                {
                    case 0:
                        activeMin = defaultMin;
                        activeMax = defaultMax;
                        break;
                    case 90:
                        activeMin.x = defaultMin.y;
                        activeMin.y = defaultMax.x;
                        activeMax.x = defaultMax.y;
                        activeMax.y = defaultMin.x;
                        break;
                    case 180:
                        activeMin.x = defaultMax.x;
                        activeMin.y = defaultMax.y;
                        activeMax.x = defaultMin.x;
                        activeMax.y = defaultMin.y;
                        break;
                    case 270:
                        activeMin.x = defaultMax.y;
                        activeMin.y = defaultMin.x;
                        activeMax.x = defaultMin.y;
                        activeMax.y = defaultMax.x;
                        break;
                }
            }
            activeMin.x = -Mathf.Abs(activeMin.x);
            activeMin.y = -Mathf.Abs(activeMin.y);
            activeMax.x = Mathf.Abs(activeMax.x);
            activeMax.y = Mathf.Abs(activeMax.y);
            UpdateDebugGrid();
        }
        else
        {
            ResetDebugGrid(MoveType.ROTATION, false, 0);
        }
    }

    private void ResetDebugGrid(MoveType type, bool horizontal, float value)
    {
        for (int i = 0; i < subBlocks.Length; i++)
        {
            int x = Mathf.RoundToInt(subBlocks[i].transform.position.x);
            int y = Mathf.RoundToInt(subBlocks[i].transform.position.y);

            if (x < Grid.cells.GetLength(0) && y < Grid.cells.GetLength(1))
            {
                Grid.cells[x, y].SetFilledState(false);
                Grid.cells[x, y].SetColor(Color.green);
                Debug.Log("falling block occupied grid cells = " + x + ", " + y);
            }
        }
        cellsReset = true;
        switch (type)
        {
            case MoveType.FALL:
                Fall();
                break;
            case MoveType.INPUT:
                InputMovement(horizontal, value);
                break;
            case MoveType.ROTATION:
                Rotate(true);
                break;
        }
    }

    private void UpdateDebugGrid()
    {
        cellsReset = false;
        for (int i = 0; i < subBlocks.Length; i++)
        {
            int x = Mathf.RoundToInt(subBlocks[i].transform.position.x);
            int y = Mathf.RoundToInt(subBlocks[i].transform.position.y);

            if (x >= Grid.cells.GetLength(0) - 1)
            {
                Debug.Log("Index x: " + x + "is invalid");
            }
            if (y >= Grid.cells.GetLength(1) - 1)
            {
                Debug.Log("Index y: " + y + "is invalid");
            }

            if (x < Grid.cells.GetLength(0) && y < Grid.cells.GetLength(1))
            {
                Grid.cells[x, y].SetFilledState(true);
                Grid.cells[x, y].SetColor(Color.red);
                Debug.Log("falling block occupied grid cells = " + x + ", " + y);
            }
        }
        blockManager.StateMachine.SetState(TheStateMachine.GameplayState.UpdatingOtherBlocks);
    }

}
