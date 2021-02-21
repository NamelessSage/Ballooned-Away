using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingService
{
    private Grid grid;

    public void GetAstarPath(GameObject playerObject, Vector3 newpos, TerrainGenerator terrain)
    {
        grid = new Grid();
        CreateWorldGrid(terrain);
        List<Node> foundPath = FindPath(playerObject.transform.position, newpos);
        if (foundPath != null)
        {
            foreach (Node node in foundPath)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.GetComponent<MeshRenderer>().material.color = Color.red;
                sphere.transform.position = new Vector3(node.X, 1.5f, node.Z);
            }

            playerObject.transform.position = new Vector3(newpos.x, playerObject.transform.position.y, newpos.z);
        }
    }

    private void CreateWorldGrid(TerrainGenerator terrain)
    {
        grid.CreateGrid(150, 150);
        AssignWalkableState(terrain);
    }

    private void AssignWalkableState(TerrainGenerator terrain)
    {
        for (int x = 0; x < grid.height; x++)
        {
            for (int z = 0; z < grid.lenght; z++)
            {
                if (terrain.GetWalkable(x, z))
                {
                    grid.setWalkable(x, z, true);
                }
            }
        }
    }

    private List<Node> FindPath(Vector3 curPos, Vector3 newPos)
    {
        Node StartNode = grid.GetNodeObject((int)curPos.x, (int)curPos.z);
        Node TargerNode = grid.GetNodeObject((int)newPos.x, (int)newPos.z);

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

            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == TargerNode)
            {
                GetFinalPath(StartNode, TargerNode);
                return grid.FinalPath;
            }

            foreach (Node n in grid.GetNeighbors(CurrentNode))
            {
                if (!n.isWalkable || ClosedList.Contains(n))
                {
                    continue;
                }

                int MoveCost = CurrentNode.gCost + GetManhatanDistance(CurrentNode, n);
                if (MoveCost < n.gCost || !OpenList.Contains(n))
                {
                    n.gCost = MoveCost;
                    n.hCost = GetManhatanDistance(n, TargerNode);
                    n.Parent = CurrentNode;
                    if (!OpenList.Contains(n))
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

    private void GetFinalPath(Node start, Node target)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = target;
        while (CurrentNode != start)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        FinalPath.Reverse();
        grid.FinalPath = FinalPath;
    }


}
