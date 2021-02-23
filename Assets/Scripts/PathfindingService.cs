﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingService
{
    // private Grid grid;

    public void GetAstarPath(GameObject playerObject, Vector3 newpos, TerrainGenerator terrain)
    {
        // CreateWorldGrid(terrain);
        List<Node> foundPath = FindPath(playerObject.transform.position, newpos, terrain);
        if (foundPath != null)
        {
            foreach (Node node in foundPath)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.GetComponent<MeshRenderer>().material.color = Color.red;
                sphere.transform.position = new Vector3(node.X, 1.5f, node.Z);
                playerObject.transform.position = new Vector3(newpos.x, playerObject.transform.position.y, newpos.z);
            }
        }
    }

    // private void CreateWorldGrid(TerrainGenerator terrain)
    // {
    //     grid.CreateGrid(150, 150);
    //     AssignWalkableState(terrain);
    // }

    // private void AssignWalkableState(TerrainGenerator terrain)
    // {
    //     for (int x = 0; x < grid.height; x++)
    //     {
    //         for (int z = 0; z < grid.lenght; z++)
    //         {
    //             if (terrain.GetWalkable(x, z))
    //             {
    //                 grid.setWalkable(x, z, true);
    //             }
    //         }
    //     }
    // }

    private List<Node> FindPath(Vector3 curPos, Vector3 newPos, TerrainGenerator terrain)
    {
        Node StartNode = new Node((int) curPos.x, (int) curPos.z, true);
        Node TargerNode = new Node((int) newPos.x, (int) newPos.z, terrain.GetWalkable((int)newPos.x, (int) newPos.z));

        List<Node> OpenList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        OpenList.Add(StartNode);
        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].fCost < CurrentNode.fCost || OpenList[i].fCost == CurrentNode.fCost && OpenList[i].hCost < CurrentNode.hCost)
                {
                    CurrentNode = OpenList[i];
                }
            }

            DeleteNode(OpenList, CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode.X == TargerNode.X && CurrentNode.Z == TargerNode.Z)
            {
                // TargerNode.Parent = CurrentNode;
                return GetFinalPath(StartNode, CurrentNode);
                // return null;
            }

            foreach (Node n in GetNeighbors(CurrentNode, terrain))
            {
                
                if (ContainsNode(ClosedList, n))
                {
                    continue;
                }

                int MoveCost = CurrentNode.gCost + GetManhatanDistance(CurrentNode, n);
                if (MoveCost < n.gCost || !ContainsNode(OpenList, n))
                {
                    n.gCost = MoveCost;
                    n.hCost = GetManhatanDistance(n, TargerNode);
                    n.Parent = CurrentNode;
                    if (!ContainsNode(OpenList, n))
                    {
                        OpenList.Add(n);
                    }
                }
            }
        }

        return null;
    }

    private int GetManhatanDistance(Node currentNode, Node neighbour)
    {
        int x = Mathf.Abs(currentNode.X - neighbour.X);
        int z = Mathf.Abs(currentNode.Z - neighbour.Z);
        return x + z;
    }

    private List<Node> GetFinalPath(Node start, Node target)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = target;
        Debug.Log("dipjfgnoisdfjghpsdf " + start.X + " " + start.Z + "   " + target.X + " " + target.Z);
        while (!(CurrentNode.X == start.X && CurrentNode.Z == start.Z))
        {
            Debug.Log(CurrentNode.X + " " + CurrentNode.Z + "   " + start.X + " " + start.Z);
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        FinalPath.Reverse();
        return FinalPath;
    }
    
    private List<Node> GetNeighbors(Node current, TerrainGenerator terrain)
    {
        List<Node> list = new List<Node>();
        //left side
        if (current.X-1>=0 && current.X-1<terrain.xSize)
        {
            //up
            if (current.Z+1<terrain.ySize && terrain.GetWalkable(current.X-1, current.Z+1))
            {
                list.Add(new Node(current.X-1, current.Z+1, true));
            }
            //center
            if (current.Z<terrain.ySize && terrain.GetWalkable(current.X-1, current.Z))
            {
                list.Add(new Node(current.X-1, current.Z, true));
            }
            //down
            if (current.Z-1>=0 && terrain.GetWalkable(current.X-1, current.Z-1))
            {
                list.Add(new Node(current.X-1, current.Z-1, true));
            }
        }
        
        //center
        if (current.X>=0 && current.X<terrain.xSize)
        {
            //up
            if (current.Z+1<terrain.ySize &&terrain.GetWalkable(current.X, current.Z+1))
            {
                list.Add(new Node(current.X, current.Z+1, true));
            }
            //down
            if (current.Z-1>=0 && terrain.GetWalkable(current.X, current.Z-1))
            {
                list.Add(new Node(current.X, current.Z-1, true));
            }
        }
        
        //right side
        if (current.X+1>=0 && current.X+1<terrain.xSize)
        {
            //up
            if (current.Z+1<terrain.ySize && terrain.GetWalkable(current.X+1, current.Z+1))
            {
                list.Add(new Node(current.X+1, current.Z+1, true));
            }
            //center
            if (current.Z<terrain.ySize && terrain.GetWalkable(current.X+1, current.Z))
            {
                list.Add(new Node(current.X+1, current.Z, true));
            }
            //down
            
            if (current.Z-1>=0 && terrain.GetWalkable(current.X + 1, current.Z - 1))
            {
                list.Add(new Node(current.X + 1, current.Z - 1, true));
            }
        }
        return list;
    }

    private bool ContainsNode(List<Node> list, Node search)
    {
        foreach (Node node in list)
        {
            if (node.X == search.X && node.Z == search.Z)
            {
                return true;
            }
        }
        return false;
    }
    
    private bool DeleteNode(List<Node> list, Node search)
    {
        foreach (Node node in list)
        {
            if (node.X == search.X && node.Z == search.Z)
            {
                list.Remove(node);
                return true;
            }
        }
        return false;
    }


}
