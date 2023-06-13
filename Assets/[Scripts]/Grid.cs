using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GridCell[,] cells;
    [SerializeField]
    private int columnCount = 24;
    [SerializeField]
    private int rowCount = 40;

    private Rect bounds;

    // Start is called before the first frame update
    void Start()
    {
        bounds.width = columnCount;
        bounds.height = rowCount;
        bounds.center = Vector2.zero;

        cells = new GridCell[columnCount, rowCount];
        for (int i = 0; i < columnCount; i++)
        {
            for (int i2 = 0; i2 < rowCount; i2++)
            {
                // cells[i, i2].position = new Vector2(bounds.min.x + 0.5f + (1.0f * i), bounds.min.y + 0.5f + (1.0f * i2)); //new GridCell(bounds.min.x + 0.5f + (1.0f * i), bounds.min.y + 0.5f + (1.0f * i2));
                cells[i, i2] = new GridCell(bounds.min.x + 0.5f + (1.0f * i), bounds.min.y + 0.5f + (1.0f * i2));
            }
        }
        Debug.Log("Cells generated");
    }



    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < columnCount; i++)
        {
            for (int i2 = 0; i2 < rowCount; i2++)
            {
                Debug.Log("Cell " + i + "," + i2 + " position = " + cells[i, i2].position);
            }
        }
    }
}
