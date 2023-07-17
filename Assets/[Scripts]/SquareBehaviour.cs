using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour
{
    public Vector2Int gridPosition = new Vector2Int();
    private MPB_Manager mpbManager;

    private void Start()
    {
        gameObject.AddComponent<MPB_Manager>();
        mpbManager = GetComponent<MPB_Manager>();
        mpbManager.ActivateHighlight();
        UpdateGridPosition();
    }

    public void UpdateGridPosition()
    {
        gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    }

    public bool CheckCollision(Vector2Int direction)
    {
        gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (gridPosition.y < 1)
            return true;
        if (direction.x != 0)
        {
            if (gridPosition.x + direction.x > 23)
            {
                return true;
                // at maximum right position
            }
            else if (gridPosition.x + direction.x < 0)
            {
                return true;
                // at maximum left position
            }
        }

        if (gridPosition.x >= Grid.cells.GetLength(0) || gridPosition.y >= Grid.cells.GetLength(1))
        {
            Debug.Log("Grid position invalid");
            return true;
        }

        if (Grid.cells[gridPosition.x + direction.x, gridPosition.y + direction.y].GetFilledState())
        {
            return true;
        }
        else
            return false;
    }

    public bool CheckFallCollision(Vector2Int direction)
    {
        gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (gridPosition.x >= Grid.cells.GetLength(0) || gridPosition.y >= Grid.cells.GetLength(1))
        {
            Debug.Log("Grid position invalid");
            return true;
        }
            if (gridPosition.y < 1)
            return true;
        else if (Grid.cells[gridPosition.x, gridPosition.y + direction.y].GetFilledState())
        {
            return true;
        }
        else
            return false;

    }

    public void SetCellFilledStatus(bool value)
    {
        if (gridPosition.x < Grid.cells.GetLength(0) && gridPosition.y < Grid.cells.GetLength(1))
            Grid.cells[gridPosition.x, gridPosition.y].SetFilledState(value);
        else
        {
            Debug.Log("GridPosition invalid");
        }
    }

    public void Unhighlight()
    {
        mpbManager.DeactivateHighlight();
    }
}
