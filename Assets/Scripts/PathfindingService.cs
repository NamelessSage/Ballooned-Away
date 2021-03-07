using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingService
{

    /// <summary>
    /// Creates shortest path from point A to point B
    /// </summary>
    /// <param name="frompos"> point A </param>
    /// <param name="newpos"> point B </param>
    /// <param name="terrain"> terrain object </param>
    /// <param name="flag"> 1-walk to Exact; 2-walk to Nearby </param>
    /// <returns></returns>
    public List<Node> GetAstarPath(Vector3 frompos, Vector3 newpos, TerrainGenerator terrain, int flag)
    {
        return FindPath(frompos, newpos, terrain, flag);

    }

    private List<Node> FindPath(Vector3 curPos, Vector3 newPos, TerrainGenerator terrain, int flag)
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

            // -----------------------------------------------------------------
            // Check if Path was Found directly to target
            if (flag == 1 && CurrentNode.X == TargerNode.X && CurrentNode.Z == TargerNode.Z)
            {
                return GetFinalPath(StartNode, CurrentNode);
            }
            // Check if Path was found nearby the target
            if (flag == 2 && terrain.IsAdjacent(CurrentNode.X, CurrentNode.Z, TargerNode.X, TargerNode.Z))
            {
                return GetFinalPath(StartNode, CurrentNode);
            }
            // -----------------------------------------------------------------

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
        FinalPath.Add(CurrentNode);
        CurrentNode = CurrentNode.Parent;

        if (CurrentNode != null)
        {
            while (!(CurrentNode.X == start.X && CurrentNode.Z == start.Z))
            {

                FinalPath.Add(CurrentNode);
                CurrentNode = CurrentNode.Parent;
            }
        }
        FinalPath.Reverse();
        return FinalPath;
    }


    /// <summary>
    /// Find all neighbors around
    /// </summary>
    private List<Node> GetNeighbors(Node current, TerrainGenerator terrain)
    {
        List<Node> list = new List<Node>();
        //left side
            //up
            if (terrain.GetWalkable(current.X-1, current.Z+1) && 
                terrain.GetWalkable(current.X, current.Z+1) && 
                terrain.GetWalkable(current.X-1, current.Z))
            {
                list.Add(new Node(current.X-1, current.Z+1, true));
            }
            //center
            if (current.Z<terrain.ySize && terrain.GetWalkable(current.X-1, current.Z))
            {
                list.Add(new Node(current.X-1, current.Z, true));
            }
            //down
            if (terrain.GetWalkable(current.X-1, current.Z-1) &&
                terrain.GetWalkable(current.X-1, current.Z) && 
                terrain.GetWalkable(current.X, current.Z-1))
            {
                list.Add(new Node(current.X-1, current.Z-1, true));
            }
        
        
        //center
            //up
            if (terrain.GetWalkable(current.X, current.Z+1))
            {
                list.Add(new Node(current.X, current.Z+1, true));
            }
            //down
            if (terrain.GetWalkable(current.X, current.Z-1))
            {
                list.Add(new Node(current.X, current.Z-1, true));
            }
            
        //right side
            //up
            if (terrain.GetWalkable(current.X+1, current.Z+1) && 
                terrain.GetWalkable(current.X+1, current.Z) && 
                terrain.GetWalkable(current.X, current.Z+1))
            {
                list.Add(new Node(current.X+1, current.Z+1, true));
            }
            //center
            if (current.Z<terrain.ySize && terrain.GetWalkable(current.X+1, current.Z))
            {
                list.Add(new Node(current.X+1, current.Z, true));
            }
            //down
            
            if (terrain.GetWalkable(current.X + 1, current.Z - 1) && 
                terrain.GetWalkable(current.X, current.Z-1) &&
                terrain.GetWalkable(current.X+1, current.Z))
            {
                list.Add(new Node(current.X + 1, current.Z - 1, true));
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
