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
        Walk_To,
        Chop_Tree,
        Open_Shop
    }

    #region Action Queue class
    private class Action
    {

        public Vector3 dst_Pos { get; }  // Destination position
        public ActionType type { get; }

        public int flag; // flags for misc, actions, default 1 (such as Walk To directly or walk to Nearby, not mandatory to use)
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
            flag = 1;
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
                Vector3 plr_pos = GetPlayerCoords();

                if (hit.collider.gameObject.CompareTag("BalloonPad"))
                {
                    if (controller.GetTerrain().IsAdjacent((int)clicked_grid_pos.x, (int)clicked_grid_pos.z, (int)plr_pos.x, (int)plr_pos.z))
                    {
                        controller.OpenShopUI();
                    }
                    else
                    {
                        AddToQue(new Action(clicked_grid_pos, ActionType.Open_Shop));
                    }
                }
                else if (controller.GetTerrain().GetWalkable((int)clicked_grid_pos.x, (int)clicked_grid_pos.z))
                {
                    AddToQue(new Action(clicked_grid_pos, ActionType.Plant_Tree));
                }
                else
                {
                    AddToQue(new Action(clicked_grid_pos, ActionType.Chop_Tree));
                }
                
            }


        }

        // -------------------------------------
        UpdateQue();
        PerformCurrentAction();
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

    // --------------------------------------
    // Que management
    #region Que methods

    // Add new action into action que
    private void AddToQue(Action newAction)
    {
        Current_Action = null;
        EndPath();
        Action_Que.Clear();

        Action_Que.Push(newAction);
        QueChanged = true;

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

            if (onTop.type.Equals(ActionType.Walk_To))
            {
                Action_Que.Clear();
            }
        }
    }

    // Update Qued action states and prepare Current action
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

                    DetermineNextStep(onTop);

                    break;
                // -------------------------------------------------------------------
                case ActionType.Chop_Tree:

                    DetermineNextStep(onTop);

                    break;
                // -------------------------------------------------------------------
                case ActionType.Open_Shop:

                    DetermineNextStep(onTop);

                    break;
                    // -------------------------------------------------------------------
            }
        }

    }

    /// <summary>
    /// Determine what to do next (applicable only for non-walking actions, such as chop tree, since we first have to walk to the tree)
    /// </summary>
    /// <param name="onTop"> peeked action at the top of action que stack </param>
    private void DetermineNextStep(Action onTop)
    {
        if (onTop.active) // If action was already reviewed, means path to it is found
        {
            Current_Action = Action_Que.Pop();
            Current_Action.Set_Origin(GetPlayerCoords());
            Current_Action.active = false; // after we walked to this action, we must begin performing it, but it is inactive anymore, since we are not chopping it yet
            Action_Que.Clear();
        }
        else // if action was encountered for the first time, walk to it firstly
        {
            Vector3 plr_pos = GetPlayerCoords();
            if (controller.GetTerrain().IsAdjacent((int)onTop.dst_Pos.x, (int)onTop.dst_Pos.z, (int)plr_pos.x, (int)plr_pos.z)) // If we are nearby the action destination, just prepare it for execution
            {
                QueChanged = true;
                onTop.active = true;
            }
            else // If we are not nearby the action destination, let's walk to it first
            {
                Current_Action = new Action(onTop.dst_Pos, ActionType.Walk_To);
                Current_Action.Set_Origin(GetPlayerCoords());
                Current_Action.flag = 2;
                onTop.active = true; // we are currently performing this action, but not directly - we first must find a path to it
            }
        }
    }

    // Perfrom current action
    private void PerformCurrentAction()
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
                        PerformAction_walk_to(Current_Action.Get_Origin(), Current_Action.dst_Pos, Current_Action.flag);

                        break;
                    // -------------------------------------------------------------------
                    case ActionType.Plant_Tree:
                        PerformAction_plant_tree_at(Current_Action.dst_Pos);
                        break;
                    // -------------------------------------------------------------------
                    case ActionType.Chop_Tree:
                        PerformAction_chop_tree_at(Current_Action.dst_Pos);
                        break;
                    // -------------------------------------------------------------------
                    case ActionType.Open_Shop:
                        controller.OpenShopUI();
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
    #endregion
    // --------------------------------------

    private void PerformAction_chop_tree_at(Vector3 pos)
    {
        //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
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
        GameObject treeObj = controller.GetTerrain().Get_Vegetation_Object_From_Grid((int)pos.x, (int)pos.z);
        tree treescript = treeObj.GetComponent<tree>();
        treescript.Perform_Chop();
        Current_Action.done = true;
    }

    private void PerformAction_plant_tree_at(Vector3 pos)
    {
        controller.GetTerrain().Spawner_Tree((int)pos.x, (int)pos.z);
        Current_Action.done = true;
    }

    /// <summary>
    /// Flags: 1-walk direcltly, 2-walk nearby
    /// </summary>
    /// <param name="flag"> 1-walk direcltly, 2-walk nearby </param>
    private void PerformAction_walk_to(Vector3 from_Pos, Vector3 to_Pos, int flag)
    {

        PathfindingService pathfinding = new PathfindingService();

        List<Node> newPath = pathfinding.GetAstarPath(from_Pos, to_Pos, controller.GetTerrain(), flag);
        
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
