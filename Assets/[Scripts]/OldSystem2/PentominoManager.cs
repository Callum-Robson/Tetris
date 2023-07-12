using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentominoManager : MonoBehaviour
{
    public Grid theGrid;
    private GameplayStateMachine stateMachine;
    [SerializeField]
    private PentominoBehaviour[] pentominoPrefabs = new PentominoBehaviour[12];
    private PentominoBehaviour activePentomino;
    // Maybe use this
    private List<PentominoBehaviour> stoppedPentominos = new List<PentominoBehaviour>();
    // Or maybe use this
    private PentominoBehaviour[,] stoppedSubBlocks = new PentominoBehaviour[24, 40];

    public Rect bounds;

    private float inputX = 0;
    private float inputY = 0;

    private bool lastInputWasX = false;

    private float fallTimer;
    private float tickTimer;
    private float elapsedTime;

    private bool fallTriggered = false;
    private bool spawnInProgress = false;
    private bool rotationTriggered = false;

    //Getters & Setters
    public GameplayStateMachine StateMachine { get { return stateMachine;} set { stateMachine = value;} }
    public float FallTimer { get {return fallTimer;} }
    public float TickTimer { get {return tickTimer;} }
    public float ElapsedTime { get {return elapsedTime;} }


    // Start is called before the first frame update
    void Start()
    {
        stateMachine = FindObjectOfType<GameplayStateMachine>();
        theGrid = FindObjectOfType<Grid>();

        bounds.width -= 1;
        bounds.height -= 1;

        //bounds.center = theGrid.bounds.center;
        Debug.Log("Bounds.Min = " + bounds.min);
        Debug.Log("Bound.Max = " + bounds.max);

        //stateMachine.SetState(GameplayStateMachine.States.Spawn);
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

        //Debug.Log("X input = " + inputX + " y input = " + inputY);

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
            //stateMachine.NextState();
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
            Debug.Log("tickTimer reset at " + Time.time);
            tickTimer = 0;
            if (inputX != 0 || inputY != 0 || rotationTriggered)
            {
                stateChanged = true;
                //stateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
            }
        }
        if (fallTimer >= 0.5f && !stateChanged)
        {
            Debug.Log("fallTimer reset at " + Time.time);
            fallTimer = 0;
            stateChanged = true;
            fallTriggered = true;
            //stateMachine.SetState(GameplayStateMachine.States.CollisionCheck);
        }
    }

    public void CheckForCollision()
    {
        //stateMachine.NextState();
        if (fallTriggered)
        {
            activePentomino.UpdateCanMove(MoveType.FALL, false, 0);
        }
        else if (rotationTriggered)
        {
            rotationTriggered = false;
            activePentomino.UpdateCanMove(MoveType.ROTATION, false, 0);
        }
        else if (inputX != 0)
        {
            activePentomino.UpdateCanMove(MoveType.INPUT, true, inputX);
        }
        else if (inputY != 0)
        {
            activePentomino.UpdateCanMove(MoveType.INPUT, false, inputY);
        }
    }

}
