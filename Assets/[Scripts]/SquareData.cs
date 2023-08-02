using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ScriptableObjects/BlockData", order = 1)]
public class SquareData : ScriptableObject
{
    public float width;
    public float height;

    public Vector2 min_XY;
    public Vector2 max_XY;


    public Vector2[] subBlockCoordinates = new Vector2[5];

}
