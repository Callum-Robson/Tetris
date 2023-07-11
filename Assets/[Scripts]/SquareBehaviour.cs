using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBehaviour : MonoBehaviour
{

    public bool CheckCollision(Vector2Int targetPosition)
    {
        if (Grid.cells[targetPosition.x, targetPosition.y].GetFilledState())
        {
            return true;
        }
        else
            return false;
    }
}
