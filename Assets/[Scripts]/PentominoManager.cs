using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentominoManager : MonoBehaviour
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
    public static bool waitingOnLineClear = false;

    public static int squaresInNeedOfFalling = 0;
    public static int squaresFinishedFalling = 0;

    public int bigSpawnBlockCount = 20;
    public bool spawnedBlockStopped = false;
    public int bigSpawnInstanced = 0;

    public bool BigSpawnNeeded = true;

    private PlayerController player;



    // Start is called before the first frame update
    void Start()
    {
        theGrid = FindObjectOfType<Grid>();

        bounds.width -= 1;
        bounds.height -= 1;
        //bounds.center = theGrid.bounds.center;

        player = FindObjectOfType<PlayerController>();

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
        // this may have somehow broke state switching backto spawn
        //squaresFinishedFalling = 0;
       // squaresFinishedFalling = 0;
        //////////////////////////////////////
        ///

        Debug.Log("Checking for filled row");
        bool lineFilled = false;
        bool valuesSet = false;
        int highestSquare = 0;
        int lowestSquare = 0;
        List<int> filledRows = new List<int>();

        //1. Get highest and lowest cells occupied by active pentomino
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

        //2. Increment highestSquare for use in for loop
        highestSquare++;
        if (highestSquare >= Grid.cells.GetLength(1))
        {
            highestSquare = Grid.cells.GetLength(1) - 1;
        }

        //3. Iterate through grid cell rows occupied by active pentomino and check for any that aren't filled, if none found, add row index to filledRows
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

        int filledRowsCount = filledRows.Count;


        //4. For every cell in a filled row, call square's Clear function
        for (int i = 0; i < filledRowsCount; i++)
        {
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                Grid.cells[i2, filledRows[i]].square.Clear();
                //Destroy(Grid.cells[i2, filledRows[i]].square.gameObject);
            }
        }

        //5. If any rows were filled, call DropAfterClear to drop higher squares to the bottom... MAYBE THIS IS WHERE THE PROBLEM IS... Since not every block that drops is higher by exactly the number of rows cleared
        if (filledRows.Count > 0)
        {
            waitingOnLineClear = true;
            DropAfterClear(filledRows.Count, filledRows[filledRows.Count-1]);
        }
        //5. Otherwise no rows were full, just move back to timer
        else
        {
            waitingOnLineClear = false;
            GameplayStateMachine.NextState();
        }

    }

    private void DropAfterClear(int linesCleared, int highestLine)
    {
        //Change this to only apply to cells above highest cleared row
        for (int i = highestLine + 1; i < Grid.cells.GetLength(1); i++)
        {
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                if (Grid.cells[i2,i].GetFilledState())
                {
                    squaresInNeedOfFalling++;
                }
            }
        }

        if (squaresFinishedFalling == 0)
        {
            waitingOnLineClear = false;

        }

        for (int i = highestLine + 1; i < Grid.cells.GetLength(1); i++)
        {
            for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
            {
                if (Grid.cells[i2, i].GetFilledState())
                {
                    Grid.cells[i2, i].square.FallAfterLineCleared(linesCleared);
                }
            }
        }



        for (int i2 = 0; i2 < Grid.cells.GetLength(0); i2++)
        {
            if (!Grid.cells[i2, 0].GetFilledState())
            {
                Debug.Log("Grid cell with y-0 not empty after line clear");
            }
        }

        GameplayStateMachine.NextState();
    }

    IEnumerator SpawnInstanceDone()
    {
        if (activePentomino.AttemptMove(Vector2Int.down))
        {
            spawnedBlockStopped = false;
            yield return false;
        }
        else
        {
            bigSpawnInstanced++;
            spawnedBlockStopped = true;
            yield return true;
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return StartCoroutine(BigSpawn());

        while (!spawnedBlockStopped)
        {
            yield return StartCoroutine(SpawnInstanceDone());
        }

        if (bigSpawnInstanced < bigSpawnBlockCount)
        {
            spawnedBlockStopped = false;
            StartCoroutine(SpawnLoop());
        }
        else
        {
            BigSpawnNeeded = false;
            spawnInProgress = false;
            player.Spawn();
            GameplayStateMachine.SetState(GameplayStateMachine.States.Spawn);
        }
    }

    IEnumerator BigSpawn()
    {
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

        bool result = true;
        yield return result;
    }

    public void StartBigSpawn()
    {
        spawnInProgress = true;
        spawnRequired = false;
        StartCoroutine(SpawnLoop());
    }
}
