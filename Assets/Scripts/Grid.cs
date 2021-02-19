using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int height;
    public int lenght;
    private Node[,] grid;
    public List<Node> FinalPath;

    public void CreateGrid(int h, int l)
    {
        height = h;
        lenght = l;
        grid = new Node[h, l];
        for (int x = 0; x < h; x++)
        {
            for (int z = 0; z < l; z++)
            {
                grid[x, z] = new Node(x, z, false);
            }
        }
    }

    public void setWalkable(int x, int z, bool isWalk)
    {
        grid[x, z].isWalkable = isWalk;
    }

    public Node GetNodeObject(int x, int z)
    {
        return grid[x, z];
    }

    public List<Node> GetNeighbors(Node current)
    {
        List<Node> list = new List<Node>();
        //left side
        if (current.X-1>=0 && current.X-1<height)
        {
            //up
            if (current.Z+1<lenght)
            {
                list.Add(grid[current.X-1, current.Z+1]);
            }
            //center
            if (current.Z<lenght)
            {
                list.Add(grid[current.X-1, current.Z]);
            }
            //down
            if (current.Z-1>=0)
            {
                list.Add(grid[current.X-1, current.Z-1]);
            }
        }
        
        //center
        if (current.X>=0 && current.X<height)
        {
            //up
            if (current.Z+1<lenght)
            {
                list.Add(grid[current.X, current.Z+1]);
            }
            //down
            if (current.Z-1>=0)
            {
                list.Add(grid[current.X, current.Z-1]);
            }
        }
        
        //right side
        if (current.X+1>=0 && current.X+1<height)
        {
            //up
            if (current.Z+1<lenght)
            {
                list.Add(grid[current.X+1, current.Z+1]);
            }
            //center
            if (current.Z<lenght)
            {
                list.Add(grid[current.X+1, current.Z]);
            }
            //down
            if (current.Z-1>=0)
            {
                list.Add(grid[current.X+1, current.Z-1]);
            }
        }
        return list;
    }
    

}
