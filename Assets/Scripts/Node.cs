using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int X;
    public bool isWalkable;
    public int Z;
    public Node Parent;

    public int gCost;
    public int hCost;

    public Node(int x, int z, bool isWalkable)
    {
        X = x;
        this.isWalkable = isWalkable;
        Z = z;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
    
    
}
