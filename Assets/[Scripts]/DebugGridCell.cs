using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGridCell : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
}
