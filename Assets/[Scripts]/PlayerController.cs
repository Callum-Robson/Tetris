using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int x = 0;
    private int jump = 0;

    public Vector2Int gridPosition = new Vector2Int();

    private bool grounded = true;
    private bool moveInProgress = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateGridPosition()
    {
        int newY = Mathf.RoundToInt(transform.position.y);
        int newX = Mathf.RoundToInt(transform.position.x);
        if (newY < 0)
            newY = 0;
        if (newX < 0)
            newX = 0;
        if (newY > 41)
            newY = 41;
        if (newX > 23)
            newY = 23;

        gridPosition = new Vector2Int(newX, newY);

    }

    public bool CheckCollision(Vector2Int direction)
    {
        int newY = Mathf.RoundToInt(transform.position.y + direction.y);
        int newX = Mathf.RoundToInt(transform.position.x + direction.x);
        if (newY < 0)
            newY = 0;
        if (newX < 0)
            newX = 0;
        if (newY > 41)
            newY = 41;
        if (newX > 23)
            newY = 23;

        gridPosition = new Vector2Int(newX, newY);


        if (Grid.cells[newX, newY].GetFilledState())
        {
            return true;
        }
        else
            return false;
    }


    // Update is called once per frame
    void Update()
    {
        if (!moveInProgress)
        {
            x = 0;
            jump = 0;

            if (Input.GetKey(KeyCode.J))
            {
                x = -1;
            }
            else if (Input.GetKey(KeyCode.L))
            {
                x = 1;
            }
            if (Input.GetKey(KeyCode.Space) && transform.position.y < 23)
            {
                jump = 1;
            }
        }      
    }

    public void Move()
    {
        moveInProgress = true;

        if (transform.position.y > 0)
        {
            if (!CheckCollision(Vector2Int.down))
            {
                transform.position += new Vector3(0, -1);
                grounded = false;
            }
            else
            {
                Debug.Log("Grounded = true");
                grounded = true;
            }
        }
        else
        {
            grounded = true;
        }

        if (!grounded)
        {
            jump = 0;
        }

        if (jump != 0 || x != 0)
        {
            if (!CheckCollision(new Vector2Int(x, jump)))
            {
                transform.position += new Vector3(x, jump, 0);
            }
        }

        moveInProgress = false;
    }
}
