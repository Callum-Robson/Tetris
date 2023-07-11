using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewManager : MonoBehaviour
{
    [SerializeField]
    private Pentomino[] pentominoPrefabs = new Pentomino[12];
    private Pentomino activePentomino;

    public Grid theGrid;
    private GameplayStateMachine stateMachine;

    public Rect bounds;

    private float inputX = 0;
    private float inputY = 0;

    private float fallTimer;
    private float tickTimer;

    private bool fallTriggered = false;
    private bool spawnInProgress = false;
    private bool rotationTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = FindObjectOfType<GameplayStateMachine>();
        theGrid = FindObjectOfType<Grid>();

        bounds.width -= 1;
        bounds.height -= 1;
        //bounds.center = theGrid.bounds.center;

        stateMachine.SetState(GameplayStateMachine.States.Spawn);
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.S))
            inputY = -1;
        else
            inputY = 0;

        if (Input.GetKeyDown(KeyCode.E))
            rotationTriggered = true;

    }

    public void SpawnBlock()
    {
        if (!spawnInProgress)
        {
            spawnInProgress = true;

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

            //4. 
            stateMachine.NextState();
        }
        spawnInProgress = false;
    }

    public void WaitForTimer()
    {
        bool stateChanged = false;
        fallTriggered = false;
        tickTimer += Time.deltaTime;
        fallTimer += Time.deltaTime;

        if (tickTimer >= 0.1f && !stateChanged)
        {
            tickTimer = 0;
            if (inputX != 0 || inputY != 0 || rotationTriggered)
            {
                stateChanged = true;
                stateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            }
        }
        if (fallTimer >= 0.5f && !stateChanged)
        {
            fallTimer = 0;
            stateChanged = true;
            fallTriggered = true;
            stateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
        }
    }

    public void CheckCollision()
    {

    }
}
