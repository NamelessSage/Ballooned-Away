using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class WorldInteractorTool : MonoBehaviour
{
    // ---------------------------------------
    // ACTION QUEUE ASSETS
    // ---------------------------------------
    private enum ActionType
    {
        Plant_Tree,
        Walk_To
    }

    #region Action Queue class
    private class Action
    {

        public Vector3 dst_Pos { get; }  // Destination position
        public ActionType type { get; }

        public bool done = false;  // Is it finished?
        public bool active = false;  // Is it being performed right now?

        private Vector3 org_Pos; // Current position

        public Action() {}

        /// <summary>
        /// Creates new action
        /// </summary>
        /// <param name="dst"> action destination position </param>
        /// <param name="t"> action type from ENUM </param>
        public Action(Vector3 dst, ActionType t)
        {
            dst_Pos = dst;
            type = t;
        }

        /// <summary>
        /// Set origin point from where to proceed to action
        /// </summary>
        /// <param name="origin"> (player location) </param>
        public void Set_Origin(Vector3 origin)
        {
            org_Pos = origin;
        }

        public Vector3 Get_Origin()
        {
            return org_Pos;
        }
    }
    #endregion

    private Stack<Action> Action_Que = new Stack<Action>(5); // Only 5 actions can be stored
    private Action Current_Action = null;
    private bool QueChanged = false;

    // ---------------------------------------
    // ---------------------------------------
    // ---------------------------------------


    [Range(0, 5)]
    public float player_Speed = 2f;

    [Range(0, 1)]
    public float precision = 0.1f;

    public GameObject GameController;
    public GameObject selector;
    
    private GameControllerScript controller;
    private List<Node> path;
    private bool pathFound = false;
    private int curNode = 0;
    private Vector3 target;
    private GameObject player;



    void Start()
    {
        controller = GameController.GetComponent<GameControllerScript>();
        player = controller.playerObj;
        selector.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Vector3 position = GetMousePos();
            //if (position.x >= 0)
            //{
            //    AdjustSelector(position);
            //}
            //else
            //{
            //    selector.SetActive(false);
            //}

            RaycastHit hit;
            Vector3 fwd = GetMousePos();
            Vector3 player = controller.playerObj.transform.position;
            float dist = Vector3.Distance(player, fwd);

            bool clicked = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (clicked)
            {
                AdjustSelector(fwd);
                Vector3 clicked_grid_pos = adjustCords(fwd);
                AddToQue(new Action(clicked_grid_pos, ActionType.Walk_To));
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Vector3 fwd = GetMousePos();
            Vector3 player = controller.playerObj.transform.position;
            float dist = Vector3.Distance(player, fwd);

            bool clicked = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (clicked)
            {
                AdjustSelector(fwd);
                Vector3 clicked_grid_pos = adjustCords(fwd);
                AddToQue(new Action(clicked_grid_pos, ActionType.Plant_Tree));
            }

            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit))
            //{
            //    if (dist <= 1.3f)
            //    {
            //        if (hit.collider.CompareTag("Tree"))
            //        {

            //            tree treescript = hit.collider.gameObject.GetComponent<tree>();
            //            Text script = hit.collider.gameObject.GetComponentInChildren<Text>();
            //            treescript.Perform_Chop();

            //        }
            //        else if (hit.collider.CompareTag("BalloonPad"))
            //        {
            //            controller.OpenShopUI();
            //        }
            //    }
            //}
        }

        // -------------------------------------
        UpdateQue();
        CheckQue();
        // -------------------------------------




        // movement block
        if (pathFound)
        {
            Vector3 dir = target - player.transform.position;
            
            player.transform.Translate(dir.normalized * player_Speed * Time.deltaTime, Space.World);
            if (Vector3.Distance(player.transform.position, target) <= precision)
            {
                GetNextNode();
            }
        }
        
    }

    private void AddToQue(Action newAction)
    {
        Current_Action = null;
        EndPath();
        Action_Que.Clear();

        Action_Que.Push(newAction);
        QueChanged = true;

    }

    private void UpdateQue()
    {

        if (Action_Que.Count > 0 && QueChanged == true)
        {
            QueChanged = false;

            Action onTop = Action_Que.Peek();
            switch (onTop.type)
            {
                // -------------------------------------------------------------------
                case ActionType.Walk_To:
                    Current_Action = Action_Que.Pop();
                    Current_Action.Set_Origin(GetPlayerCoords());
                    Current_Action.active = false;

                    break;
                // -------------------------------------------------------------------
                case ActionType.Plant_Tree:
                    if (onTop.active) // if PlantTree action was reviewd before, means it had it's path already assigned
                    {
                        Current_Action = Action_Que.Pop();
                        Current_Action.Set_Origin(GetPlayerCoords());
                        Current_Action.active = false;
                        Action_Que.Clear();
                    }
                    else // if action was encountered for the first time, walk to it firstly
                    {
                        Current_Action = new Action(onTop.dst_Pos, ActionType.Walk_To);
                        Current_Action.Set_Origin(GetPlayerCoords());
                        onTop.active = true;
                    }


                    break;
                // -------------------------------------------------------------------
            }
        }

    }

    private void ResetQue()
    {
        Current_Action = null;
        Action_Que.Clear();
    }

    private void EmptyQue()
    {
        if (Action_Que.Count > 0)
        {
            Action onTop = Action_Que.Peek();

            if (!onTop.type.Equals(ActionType.Plant_Tree))
            {
                Action_Que.Clear();
            }
        }
    }

    private void CheckQue()
    {
        if (Current_Action != null)
        {
            if (Current_Action.done == false && Current_Action.active == false)
            {
                Current_Action.active = true;
                switch (Current_Action.type)
                {
                    // -------------------------------------------------------------------
                    case ActionType.Walk_To:
                        movePlayer(Current_Action.Get_Origin(), Current_Action.dst_Pos);

                        break;
                    // -------------------------------------------------------------------
                    case ActionType.Plant_Tree:
                        Debug.Log("Planting tree");
                        Current_Action.done = true;
                        
                        break;
                    // -------------------------------------------------------------------
                }

            }

            if (Current_Action != null && Current_Action.done)
            {
                Current_Action = null;
                QueChanged = true;

                EmptyQue();
            }

        }
    }




    private void movePlayer(Vector3 from_Pos, Vector3 to_Pos)
    {

        PathfindingService pathfinding = new PathfindingService();

        List<Node> newPath = pathfinding.GetAstarPath(from_Pos, to_Pos, controller.GetTerrain());
        
        if (newPath != null)
        {
            if (newPath.Count > 0)
            {
                path = newPath;

                pathFound = true;
                float y = 0;
                if (controller.GetTerrain().Get_Terrian_Object_From_Grid(path[0].X, path[0].Z).CompareTag("Rock"))
                {
                    y = 1.7f;
                }
                else
                {
                    y = 1.2f;
                }
                target = new Vector3(path[0].X, y, path[0].Z);
            }
        }
        else // If we can't walk there, means we can't do anything so we kill everything
        {
            ResetQue();
        }
        
    }
    
    private void EndPath()
    {
        if (path != null)
        {
            pathFound = false;
            curNode = 0;
            path.Clear();
        }


    }

    private void GetNextNode()
    {
        if (path!=null)
        {
            if (curNode >= path.Count-1)
            {

                EndPath();
                if (Current_Action != null && Current_Action.type.Equals(ActionType.Walk_To))
                {
                    Current_Action.done = true;
                }
                return;
            }
            curNode++;
            float y = 0;
            if (controller.GetTerrain().Get_Terrian_Object_From_Grid(path[curNode].X, path[curNode].Z).CompareTag("Rock"))
            {
                y = 1.7f;
            }
            else
            {
                y = 1.2f;
            }
            target = new Vector3(path[curNode].X, y, path[curNode].Z);
        }
    }




    private Vector3 GetMousePos()
    {
        Vector3 clickPosition = -Vector3.one;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            clickPosition = hit.point;
        }

        return clickPosition;
    }

    private Vector3 adjustCords(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + 0.01f);
        int y = Mathf.RoundToInt(pos.y + 0.01f);
        int z = Mathf.RoundToInt(pos.z + 0.01f);

        return new Vector3(x, y, z);
    }

    private Vector3 GetPlayerCoords()
    {
        return adjustCords(player.transform.position);
    }

    private void AdjustSelector(Vector3 position)
    {
        GameObject target = controller.GetTerrainObjectFromPosition(position);
        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 newSelectroPos = new Vector3(targetPos.x,
                                                            targetPos.y + (target.transform.localScale.y / 2) + 0.005f,
                                                            targetPos.z);
            selector.SetActive(true);
            selector.transform.position = newSelectroPos;
        }

    }
}
