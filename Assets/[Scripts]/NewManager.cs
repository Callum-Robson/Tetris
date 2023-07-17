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
}
