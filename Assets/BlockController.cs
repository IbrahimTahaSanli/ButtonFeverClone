using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockController : MonoBehaviour
{
    public static BlockController instance;

    [HideInInspector] private List<Block> freeBlocks;
    [SerializeField] private int FreeBlockCount {
        get => FreeBlockPositions.Count;
    }
    [SerializeField] private List<Vector3> FreeBlockPositions;

    [SerializeField] private List<GameObject> Blocks;

    public BlockController()
    {
        if (instance != null)
            return;

        instance = this;

    }

    public int IsFree()
    {
        if (freeBlocks.Count < FreeBlockCount)
            return freeBlocks.Count;
        return -1;
    }

    public void GetNewBlock()
    {
        int freeBlockPos = IsFree();
        if (freeBlockPos == -1)
            return;

        GameObject newBlock = Instantiate(Blocks[Random.Range(0, Blocks.Count)]);
        newBlock.transform.position = FreeBlockPositions[freeBlockPos];
        freeBlocks.Add( newBlock.GetComponent<Block>());
    }

    public void RemoveBlock(Block block)
    {
        freeBlocks.Remove(block);

        for (int i = 0; i < freeBlocks.Count; i++)
            freeBlocks[i].transform.position = FreeBlockPositions[i];
    }

    public void PutBlock(Block block)
    {
        int freeBlockPos = IsFree();
        if (freeBlockPos == -1)
            return;

        block.transform.position = FreeBlockPositions[freeBlockPos];
        freeBlocks.Add( block);
    }

    public void UpgradeBlock(Block inFree, Block inGrid)
    {
        if (inFree.type != inGrid.type)
            return;

        int newBlockIndex;
        Array array = Enum.GetValues(typeof(Block.Blocks));
        for (newBlockIndex = 0; newBlockIndex < array.Length; newBlockIndex++)
            if ((Block.Blocks)array.GetValue(newBlockIndex) > inFree.type)
                break;

        GameObject newBlock = Instantiate(Blocks[newBlockIndex]);
        newBlock.transform.position = FreeBlockPositions[freeBlocks.IndexOf(inFree)];
        freeBlocks[freeBlocks.IndexOf(inFree)] = (newBlock.GetComponent<Block>());

        Destroy(inFree.gameObject);
        if (inGrid.IsInGrid)
            GridController.instance.RemoveFromGrid(inGrid);
        else
            freeBlocks.Remove(inGrid);
        Destroy(inGrid.gameObject);
    }

    public void Awake()
    {
        freeBlocks = new List<Block>();

    }

    public void Start()
    {
        GetNewBlock();
        GetNewBlock();
        GetNewBlock();
    }
}
