using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pentomino : MonoBehaviour
{
    private NewManager newManager;
    public List<SquareBehaviour> squares = new List<SquareBehaviour>();
    public BlockData blockData;
    private bool stopped = false;
    private bool rotated = false;

    // Start is called before the first frame update
    void Start()
    {
        #region
        //  For quickly modifying prefabs   //
        //string letter = gameObject.name[gameObject.name.Length-1].ToString();
        //Debug.Log("Index = " + (gameObject.name.Length - 1));
        //Debug.Log(letter);
        //blockData = Resources.Load<BlockData>("BlockData/" + letter + " Block");
        //SubBlockBehaviour[] subBlocks = GetComponentsInChildren<SubBlockBehaviour>();
        //foreach (SubBlockBehaviour block in subBlocks)
        //{
        //    block.gameObject.AddComponent<SquareBehaviour>();
        //    Destroy(block.gameObject.GetComponent<BoxCollider>());
        //    Destroy(block);
        //}
        //squares = GetComponentsInChildren<SquareBehaviour>();
        #endregion

        newManager = FindObjectOfType<NewManager>();
    }

    // Checks if the destination space for each square is empty, if any one is found to be filled, stop checking and set stopped to true;
    // If no filled cells are found, move the pentomino.
    public bool AttemptMove(Vector2Int direction)
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.WaitForLineClear)
        {
            return false;
        }
        bool collided = false;

        //Temporarily set all of the grid cells occupied by this pentomino to empty
        //This is so that the squares will be free to move into any spot that was filled by another of this pentomino's squares
        foreach (SquareBehaviour square in squares)
        {
            square.SetCellFilledStatus(false);
        }

        if (InputManager.inputType != InputType.fall)
        {
            foreach (SquareBehaviour square in squares)
            {
                if (square.CheckCollision(direction))
                {
                    collided = true;
                    if (direction.y != 0)
                    {
                        stopped = true;
                        if (rotated)
                        {
                            Debug.Log("Rotated Pentomino Stopped");
                        }
                        Stop();
                        NewManager.spawnRequired = true;
                        return false;
                    }
                    break;
                }
            }
            if (collided == false)
            {
                transform.position += new Vector3(direction.x, direction.y);
            }
            // ADD CHECK FOR STOPPED?
            foreach (SquareBehaviour square in squares)
            {
                square.UpdateGridPosition();
                square.SetCellFilledStatus(true);
            }
            if (!stopped)
                GameplayStateMachine.NextState();
            return true;

        }
        else
        {
            foreach (SquareBehaviour square in squares)
            {
                if (square.CheckFallCollision(Vector2Int.down))
                {
                    collided = true;

                    stopped = true;
                    if (rotated)
                    {
                        Debug.Log("Rotated Pentomino Stopped");
                    }
                    Stop();
                    NewManager.spawnRequired = true;
                    return false;
                }
            }
            if (collided == false)
            {
                transform.position += new Vector3(0, -1);
            }

            // ADD CHECK FOR STOPPED?
            foreach (SquareBehaviour square in squares)
            {
                square.UpdateGridPosition();
                square.SetCellFilledStatus(true);
            }
            if (!stopped)
                GameplayStateMachine.NextState();
            return true;
        }

    }

    public bool AttemptRotation(bool clockwise)
    {
        if (GameplayStateMachine.CurrentState == GameplayStateMachine.States.WaitForLineClear)
        {
            return false;
        }
        foreach (SquareBehaviour square in squares)
        {
            square.SetCellFilledStatus(false);
        }
        float rotationAmount = 0;

        if (clockwise)
            rotationAmount = -90;
        else
            rotationAmount = 90;

        //Rotate the pentomino
        transform.Rotate(0, 0, rotationAmount);

        rotated = true;

        //Check if any of the new positions of eac
        foreach (SquareBehaviour square in squares)
        {
            bool collision = square.CheckCollision(Vector2Int.zero);//new Vector2Int((int)square.transform.position.x, (int)square.transform.position.y));
            if (collision)
            {
                rotated = false;
                transform.Rotate(0, 0, -rotationAmount);
                //return false;
            }
        }

        foreach (SquareBehaviour square in squares)
        {
            square.UpdateGridPosition();
            square.SetCellFilledStatus(true);
        }
        if (!stopped)
            GameplayStateMachine.NextState();
        return true;
    }

    private void Stop()
    {

        foreach (SquareBehaviour square in squares)
        {
            square.UpdateGridPosition();
            square.SetCellFilledStatus(true);
            square.AssignToCell();
        }
        if (rotated)
        {
            Debug.Log("Rotated pentomino stopeed");
        }
        foreach (SquareBehaviour square in squares)
        {
            square.Unhighlight();
        }
        newManager.CheckForFilledRow();
    }
    //TODO: Create a function to check each row occupied by a stopped pentomino, to see if all cells are filled, then clear the row.

}
