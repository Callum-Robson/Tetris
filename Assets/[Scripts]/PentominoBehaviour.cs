using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentominoBehaviour : MonoBehaviour
{
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
        if (cellsReset)
        {
            // movement

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
        switch(type)
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
