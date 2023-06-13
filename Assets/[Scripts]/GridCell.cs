using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector2 position;
    private bool isFilled;

    public GridCell(float x, float y)
    {
        position.x = x; position.y = y;
    }


    public void SetIsFilled(bool value)
    {
        isFilled = value;
    }
}
