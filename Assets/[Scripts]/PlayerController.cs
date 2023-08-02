using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private int x = 0;
    private int jump = 0;

    public Vector2Int gridPosition = new Vector2Int();

    private bool grounded = true;
    private bool moveInProgress = false;
    private bool isSpawned = false;
    private int jumpCount = 0;
    private bool jumped = false;

    private bool jumpTriggered = false;
    private bool jumpExecuted = false;
    private bool doubleJumpTriggered = false;
    private bool doubleJumpExecuted = false;

    private PentominoManager pManager;

    // Start is called before the first frame update
    void Start()
    {
        pManager = FindObjectOfType<PentominoManager>();   
    }

    public void UpdateGridPosition()
    {
        if (isSpawned)
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
    }

    public bool CheckCollision(Vector2Int direction)
    {
        if (isSpawned)
        {
            int newY = Mathf.RoundToInt(transform.position.y + direction.y);
            int newX = Mathf.RoundToInt(transform.position.x + direction.x);
            if (newY < 0)
            {
                newY = 0;
                return true;
            }
            if (newX < 0)
            {
                newX = 0;
                return true;
            }
            if (newY > 41)
            {
                newY = 41;
                return true;
            }
            if (newX > 23)
            {
                newY = 23;
                return true;
            }


            gridPosition = new Vector2Int(newX, newY);


            if (Grid.cells[newX, newY].GetFilledState())
            {
                return true;
            }
            else
                return false;
        }
        else return false;
       
    }


    // Update is called once per frame
    void Update()
    {
        if (isSpawned)
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpExecuted)
                doubleJumpTriggered = true;

            if (Input.GetKeyDown(KeyCode.Space) && !jumpTriggered)
                jumpTriggered = true;

            if (!moveInProgress)
            {
                x = 0;
                jump = 0;

                if (Input.GetKey(KeyCode.J))
                {
                    x = -1;
                    spriteRenderer.flipX = false;
                }
                else if (Input.GetKey(KeyCode.L))
                {
                    x = 1;
                    spriteRenderer.flipX = true;
                }
                if (Input.GetKey(KeyCode.Space) && transform.position.y < 23)
                {
                    jump = 1;
                }
            }
        }
    }

    public void Move()
    {
        if (isSpawned)
        {
            moveInProgress = true;

            Vector2Int moveDirection = Vector2Int.zero;

            moveDirection.x = x;

            if(jumpTriggered)
            {
                moveDirection.y = 1;
            }

            // If player not on lowest tile, check if the tile below is filled, if not, fall.
            if (transform.position.y > 0)
            {
                if (!CheckCollision(Vector2Int.down))
                {
                    transform.position += new Vector3(0, -0.5f);
                    grounded = false;
                }
                else
                {
                    Debug.Log("Grounded = true");
                    grounded = true;
                }
            }
            // Player on lowest tile
            else
            {
                grounded = true;
            }


            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                if (!CheckCollision(moveDirection))
                {
                    transform.position += new Vector3(moveDirection.x, moveDirection.y, 0);
                }
            }

            jumpExecuted = true;

            if (doubleJumpTriggered && transform.position.y < 23)
            {
                doubleJumpExecuted = true;
                doubleJumpTriggered = false;
                Invoke(nameof(DoubleJump), 0.1f);
            }

            jumpTriggered = false;

            moveInProgress = false;
        }
    }

    public void DoubleJump()
    {
        if (!CheckCollision(Vector2Int.up))
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    public void Spawn()
    {
        //1.    Pick random x value for spawn position
        float randomX = (int)Random.Range(pManager.bounds.xMin, pManager.bounds.xMax + 1);

        transform.position = new Vector3(randomX, pManager.bounds.max.y);
        isSpawned = true;
    }
}
