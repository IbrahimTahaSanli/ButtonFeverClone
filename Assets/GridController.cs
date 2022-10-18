using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridController : MonoBehaviour
{

    public static GridController instance;

    public GridController()
    {
        if (instance != null)
            return;

        instance = this;

        ActiveBlocks = new HashSet<Block>();
        Blocks = new List<Block>();
    }

    [SerializeField] public Vector3 gridSize;

    [SerializeField] public Vector3 nodeOffset;
    [SerializeField] public GameObject keyHolder;

    [SerializeField] public Vector3 BlockOffset;

    [SerializeField] private Color DisabledBlockColor;
    [SerializeField] private Color KeyboardEnteranceColor;

    [SerializeField] private float RowMultiplier;


    [HideInInspector] public float TotalRowMultiplier;
    [HideInInspector] public HashSet<Block> ActiveBlocks;
    [HideInInspector] public List<Block> Blocks;



    public void Awake()
    {
        SetGrid();
    }

    public Vector2 LocalSpaceToGridCell(Vector3 vec)
    {
        return new Vector2(Mathf.Floor((vec.x + 0.5f) / (1 / gridSize.x)), Mathf.Floor( (-vec.y + 0.5f) / (1 /  gridSize.y)));
    }

    public Vector3 GridCellToLocalSpace(Vector2 vec)
    {
        return new Vector3((vec.x / gridSize.x) - 0.5f, -(vec.y / gridSize.y) + 0.5f , BlockOffset.z );
    }

    public Vector3 GridCellToWorldSpace(Vector2 vec)
    {
        return LocalSpaceToWorldSpace(GridCellToLocalSpace(vec));
    }

    public Vector3 LocalSpaceToWorldSpace(Vector3 vec)
    {
        return this.transform.TransformPoint(vec);
    }

    private Block[,] Grid;
    public bool CheckIsEmpty(Block block, Vector2 pos)
    {

        bool[,] collision = block.gridColiision;
        for (int i = 0; i < collision.GetLength(0); i++)
            for (int j = 0; j < collision.GetLength(1); j++)
                if (((pos.x + j < 0 )|| (pos.y + i < 0) || ((pos.x + j )> (gridSize.x - 1)) || ((pos.y + i) > (gridSize.y - 1))) 
                    || (collision[i, j] && Grid[(int)pos.x + j, (int)pos.y + i] != null))
                    return false;

        return true;
    }

    public void SetBlock(Block block, Vector2 pos)
    {
        if (!CheckIsEmpty(block, pos))
            return;

        bool[,] collision = block.gridColiision;

        for (int i = 0; i < collision.GetLength(0); i++)
            for (int j = 0; j < collision.GetLength(1); j++)
                if (collision[i, j])
                    Grid[(int)pos.x + j, (int)pos.y + i] = block;
                
        block.transform.position = GridCellToWorldSpace(pos);

        Vector3 ex = GetMaxBounds(block.gameObject).extents;
        ex.Set(ex.x, -ex.y, 0);
        block.transform.position += ex;

        block.PosInGrid = pos;
        block.IsInGrid = true;

        Blocks.Add(block);

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        ActiveBlocks.Clear();

        foreach (Block bl in Blocks)
            bl.SetColor(DisabledBlockColor);
        
        List<Vector2> WillSearchNode = new List<Vector2>();
        WillSearchNode.Add(gridSize - Vector3.one);
        int currentIndex = 0;

        while(currentIndex != WillSearchNode.Count)
        {
            Vector2 currentNode = WillSearchNode[currentIndex];
            for (int i = 0; i < currentIndex; i++)
            {
                if (currentNode == WillSearchNode[i])
                {
                    
                    goto ENDOFSEARCH;
                }

            }


            if (Grid[(int)currentNode.x, (int)currentNode.y] == null)
                goto ENDOFSEARCH;


            if (currentNode.x - 1 >= 0)
                WillSearchNode.Add(new Vector2(currentNode.x - 1, currentNode.y));

            if (currentNode.y - 1 >= 0)
                WillSearchNode.Add(new Vector2(currentNode.x, currentNode.y - 1));

            if (currentNode.x + 1 < gridSize.x)
                WillSearchNode.Add(new Vector2(currentNode.x + 1, currentNode.y));

            if (currentNode.y + 1 < gridSize.y)
                WillSearchNode.Add(new Vector2(currentNode.x, currentNode.y + 1));

            ActiveBlocks.Add(Grid[(int)currentNode.x, (int)currentNode.y]);

            ENDOFSEARCH:
            currentIndex++;
        }

        foreach (Block bl in ActiveBlocks)
            bl.SetColor();

        TotalRowMultiplier = GetRowMultiplier();
    }


    private float GetRowMultiplier()
    {
        float rowMultiplier = 1;

        for(int i = 0; i < gridSize.y; i++)
        {
            for(int j = 0; j < gridSize.x; j++)
                if (Grid[j, i] == null)
                    goto ROWCHECKEND;

            rowMultiplier += RowMultiplier;

        ROWCHECKEND:;
        }

        return rowMultiplier;
            
    }


    private Bounds GetMaxBounds(GameObject g)
    {
        var b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    public void RemoveFromGrid(Block block)
    {
        Vector2 pos = block.PosInGrid;
        
        bool[,] collision = block.gridColiision;

        for (int i = 0; i < collision.GetLength(0); i++)
            for (int j = 0; j < collision.GetLength(1); j++)
                if (collision[i, j])
                    Grid[(int)pos.x + j, (int)pos.y + i] = null;

        block.IsInGrid = false;
        Blocks.Remove(block);

        UpdateGrid();
    }

    private void SetGrid()
    {
        this.transform.localScale = gridSize;
        this.transform.position = Vector3.zero;

        for(int i = 0; i < gridSize.x; i++)
            for(int j = 0; j < gridSize.y; j++)
            {
                GameObject obj = Instantiate(keyHolder);
                obj.transform.parent = this.transform;
                obj.transform.position = new Vector3( 
                    ( i - ( gridSize.x / 2)) + nodeOffset.x,
                    ( j - ( gridSize.y / 2)) + nodeOffset.y,
                    nodeOffset.z);
                if (i == gridSize.x - 1 && j == 0)
                    obj.GetComponent<MeshRenderer>().material.color = KeyboardEnteranceColor;
            }

        Grid = new Block[(int)gridSize.x, (int)gridSize.y];
    }

}
