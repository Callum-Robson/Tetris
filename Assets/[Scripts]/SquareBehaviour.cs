using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour
{
    public Vector2Int gridPosition = new Vector2Int();

    private void Start()
    {
        UpdateGridPosition();
    }

    public void UpdateGridPosition()
    {
        gridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        
    }

    public bool CheckCollision(Vector2Int direction)
    {
        gridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        if (gridPosition.y < 1)
            return true;
        if (direction.x != 0)
        {
            if (gridPosition.x > 22)
            {
                return true;
                // at maximum right position
            }
            else if (gridPosition.x < 1)
            {
                return true;
                // at maximum left position
            }
        }

        if (Grid.cells[gridPosition.x + direction.x, gridPosition.y + direction.y].GetFilledState())
        {
            return true;
        }
        else
            return false;
    }

    public void SetCellFilledStatus(bool value)
    {
        Grid.cells[gridPosition.x, gridPosition.y].SetFilledState(value);
    }
}
