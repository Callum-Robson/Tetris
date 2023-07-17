using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewManager : MonoBehaviour
{
    [SerializeField]
    private Pentomino[] pentominoPrefabs = new Pentomino[12];
    private Pentomino activePentomino;
    private List<Pentomino> otherPentominos = new List<Pentomino>();

    public Grid theGrid;
    private GameplayStateMachine stateMachine;

    public Rect bounds;
    public static bool spawnInProgress = false;
    public static bool spawnRequired = true;


    // Start is called before the first frame update
    void Start()
    {
        theGrid = FindObjectOfType<Grid>();

        bounds.width -= 1;
        bounds.height -= 1;
        //bounds.center = theGrid.bounds.center;

        GameplayStateMachine.NextState();
    }


    public void SpawnBlock()
    {
        if (!spawnInProgress)
        {
            spawnInProgress = true;
            spawnRequired = false;

            if (activePentomino != null)
            {
                otherPentominos.Add(activePentomino);
            }

            //1.    Pick random index for random Pentomino
            int randomBlock = Random.Range(0, 12);

            //2.    Pick random x value for spawn position
            float randomX = (int)Random.Range(bounds.xMin, bounds.xMax + 1);
            
            //2-A.  Calculate minimum and maxium x position selected block can occupy while remaining in bounds.
            float minPositionX = -pentominoPrefabs[randomBlock].blockData.min_XY.x + bounds.min.x;
            float maxPositionX = bounds.max.x - pentominoPrefabs[randomBlock].blockData.max_XY.x;
            
            //2-B.  Y Position for spawn
            float ySpawn = bounds.max.y - pentominoPrefabs[randomBlock].blockData.max_XY.y;
            
            //2-C.  If randomX is out of bounds, set it to minPositionX or maxPositionX, whichever is closest.
            if (minPositionX > randomX)
                randomX = minPositionX;
            else if (maxPositionX < randomX)
                randomX = maxPositionX;

            Vector3 spawnPosition = new Vector3(randomX, ySpawn, 0);

            //3.    Instantiate selected block prefab at generated position, with default rotation, as fallingBlock. Add to activeBlocks.
            activePentomino = Instantiate(pentominoPrefabs[randomBlock], spawnPosition, pentominoPrefabs[randomBlock].transform.rotation);

            //4.    Block spawned, go to next state
            GameplayStateMachine.NextState();
            spawnInProgress = false;
        }
    }

    public void CheckCollision()
    {
        if (InputManager.inputType == InputType.fall)
        {
            activePentomino.AttemptMove(Vector2Int.down);
        }
        else if (InputManager.inputType == InputType.moveX)
        {
            activePentomino.AttemptMove(new Vector2Int(InputManager.inputX, 0));
        }
        else if (InputManager.inputType == InputType.moveY)
        {
            activePentomino.AttemptMove(Vector2Int.down);
        }
        else if (InputManager.inputType == InputType.rotate)
        {
            activePentomino.AttemptRotation(true);
        }
    }


    public void CheckForFilledRow()
    {
        bool lineFilled = false;
        bool valuesSet = false;
        int highestSquare = 0;
        int lowestSquare = 0;
        List<int> filledRows = new List<int>();

        for(int i = 0; i < 5; i++)
        {
            if (!valuesSet)
            {
                highestSquare = activePentomino.squares[i].gridPosition.y;
                lowestSquare = activePentomino.squares[i].gridPosition.y;
                valuesSet = true;
            }
            else
            {
                if (activePentomino.squares[i].gridPosition.y < lowestSquare)
                    lowestSquare = activePentomino.squares[i].gridPosition.y;

                if (activePentomino.squares[i].gridPosition.y > highestSquare)
                    highestSquare = activePentomino.squares[i].gridPosition.y;
            }
        }

        for (int i = lowestSquare; i < highestSquare; i++)
        {
            int filledCells = 0;
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                if (!Grid.cells[i2,i].GetFilledState())
                {
                    break;
                }
                else
                {
                    filledCells++;
                }
            }
            if (filledCells == Grid.cells.GetLength(0))
            {
                filledRows.Add(i);
                Debug.Log("Row #" + i + " filled");
            }
        }

        for (int i = 0; i < filledRows.Count; i++)
        {
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                Destroy(Grid.cells[i2, filledRows[i]].square.gameObject);
            }
        }


        GameplayStateMachine.NextState();
    }
}
