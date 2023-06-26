using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector2 position;
    private bool isFilled;
    public DebugGridCell debugCell;
    private Bounds bounds;

    public GridCell(float x, float y)
    {
        position.x = x; position.y = y;
        bounds.center = position;
        bounds.size = Vector3.one;

    }

    public void InitializeDebugCell()
    {
        debugCell.spriteRenderer.bounds = bounds;
        debugCell.spriteRenderer.color = Color.green;
        debugCell.transform.position = new Vector3(position.x, position.y, -1);
    }

    public void SetColor(Color newColor)
    {
        debugCell.spriteRenderer.color = newColor;
        if (newColor.r == 1)
            Debug.Log("cell at position " + position + "  set color to red");
        else
            Debug.Log("cell at position " + position + "  set color to green");
    }

    public void SetFilledState(bool value)
    {
        isFilled = value;
        Debug.Log("cell at position " + position + "  set isFilled to " + value);
    }

    public bool GetFilledState()
    {
        return isFilled;
    }

}
