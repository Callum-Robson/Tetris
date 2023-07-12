using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pentomino : MonoBehaviour
{
    public SquareBehaviour[] squares;
    public BlockData blockData;
    private bool stopped = false;

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


    }

    // Checks if the destination space for each square is empty, if any one is found to be filled, stop checking and set stopped to true;
    // If no filled cells are found, move the pentomino.
    public void AttemptMove(Vector2Int direction)
    {
        bool collided = false;

        //Temporarily set all of the grid cells occupied by this pentomino to empty
        //This is so that the squares will be free to move into any spot that was filled by another of this pentomino's squares
        foreach (SquareBehaviour square in squares)
        {
            square.SetCellFilledStatus(false);
        }

        foreach (SquareBehaviour square in squares)
        {
            if (square.CheckCollision(direction))
            {
                collided = true;
                if (direction.y != 0)
                {
                    stopped = true;
                    NewManager.spawnRequired = true;
                }
                break;
            }
        }
        if (collided == false)
        {
            transform.position += new Vector3(direction.x, direction.y);
        }

        foreach (SquareBehaviour square in squares)
        {
            square.SetCellFilledStatus(true);
        }
        
        GameplayStateMachine.NextState();
    }

    public void AttemptRotation(bool clockwise)
    {
        float rotationAmount = 0;

        if (clockwise)
            rotationAmount = -90;
        else
            rotationAmount = 90;

        //Rotate the pentomino
        transform.Rotate(0, 0, rotationAmount);

        //Check if any of the new positions of eac
        foreach (SquareBehaviour square in squares)
        {
            bool collision = square.CheckCollision(new Vector2Int((int)square.transform.position.x, (int)square.transform.position.y));
            if (collision)
            {
                transform.Rotate(0, 0, -rotationAmount);
                break;
            }
        }
    }

    //TODO: Create a function to check each row occupied by a stopped pentomino, to see if all cells are filled, then clear the row.
}
