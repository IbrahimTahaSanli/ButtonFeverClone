using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block: MonoBehaviour
{
    public enum Blocks
    {
        Block1 = 1,
        Block2 = 2,
        Block4 = 4,
        Block8 = 8
    }

    [SerializeField] public Blocks type;
    
    [HideInInspector] private Color color
    {
        get
        { 
            switch (type)
            {
                case Blocks.Block1:
                    return new Color(1, 0, 0);
                case Blocks.Block2:
                    return new Color(0, 0, 1);
                case Blocks.Block4:
                    return new Color(0, 1, 0);
                case Blocks.Block8:
                    return new Color(0.5f, 1, 0);
            }

            return new Color(0, 0, 0 , 0);
        }
}
    [HideInInspector] public bool IsInGrid = false;
    [HideInInspector] public Vector2 PosInGrid;

    public bool[,] gridColiision
    {
        get
        {
            switch (type)
            {
                case Blocks.Block1:
                    return new bool[,] { { true } };
                case Blocks.Block2:
                    return new bool[,] { { true, true } };
                case Blocks.Block4:
                    return new bool[,] { { true, true, true } };
                case Blocks.Block8:
                    return new bool[,] { { false, true, false }, { true, true, true } };
            }

            return null;
        }
    }

    public void OnEnable()
    {
        SetColor();
    }

    public void SetColor()
    {
        foreach(Transform child in this.transform)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>(); 
            if(renderer != null)
            {
                renderer.material.color = color;
            }

        }
    }

    public void SetColor(Color col)
    {
        foreach (Transform child in this.transform)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.material.color = col;
        }
    }


}
