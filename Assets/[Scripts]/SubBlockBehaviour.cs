using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBlockBehaviour : MonoBehaviour
{
    private Vector2 gridPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePositionData()
    {
        Grid.cells[(int)gridPosition.x, (int)gridPosition.y].isFilled = false;
        gridPosition.x = Mathf.RoundToInt(transform.position.x + 11.5f);
        gridPosition.y = Mathf.RoundToInt(transform.position.y + 19.5f);
        Grid.cells[(int)gridPosition.x, (int)gridPosition.y].isFilled = true;
    }
}
