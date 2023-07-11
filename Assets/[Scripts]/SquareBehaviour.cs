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

    public bool CheckCollision(Vector2Int targetPosition)
    {
        if (Grid.cells[targetPosition.x, targetPosition.y].GetFilledState())
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
