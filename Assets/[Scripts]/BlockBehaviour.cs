using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    public BlockData blockData;
    public BlockManager blockManager;
    public bool stopped = false;
    public float sizeFactor;

    private void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        sizeFactor = blockManager.sizeFactorY;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -blockData.maxAllowablePosition.y * sizeFactor && !stopped)
        {
            stopped = true;
            blockManager.ResetKeys();
        }
    }
}
