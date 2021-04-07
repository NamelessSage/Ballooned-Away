using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
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
        Open_Shop,
        PickUpShoorm
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
        /// Creates new action, default flag value 1
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
        /// Creates new action with your define flag
        /// </summary>
        /// <param name="dst"> action destination position </param>
        /// <param name="t"> action type from ENUM </param>
        public Action(Vector3 dst, ActionType t, int flag)
        {
            this.flag = flag;
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

    private Stack<Action> Action_Que = new Stack<Action>(3); // Only 3 actions can be stored
    private Action Current_Action = null;
    //private Action Previous_Root_Action = null;
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
    public GameObject EventSysObj;
    
    private GameControllerScript controller;
    private List<Node> path;
    private bool pathFound = false;
    private int curNode = 0;
    private Vector3 target;
    private GameObject player;
    private EventSystem EventSys;



    void Start()
    {
        controller = GameController.GetComponent<GameControllerScript>();
        EventSys = EventSysObj.GetComponent<EventSystem>();
        player = controller.playerObj;
        selector.SetActive(false);
        
    }

    void Update()
    {
        // -------------------------------------
        #region Mouse Input
        bool leftBtn = Input.GetMouseButtonDown(0);
        bool rightBtn = Input.GetMouseButtonDown(1);
        
        if (!EventSys.currentSelectedGameObject && (leftBtn || rightBtn)) // If right or left button was smacked AND wasnt a UI smack
        {
            RaycastHit hit;
            Vector3 clickPosition;
            Vector3 clickPositionOnGrid;
            Vector3 playerPos = GetPlayerPosition_Adjusted();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool clicked = Physics.Raycast(ray, out hit);

            if (clicked)
            {
                clickPosition = hit.point;
                clickPositionOnGrid = adjustCords(hit.point);
                AdjustSelector(clickPosition);

                TerrainGenerator terrain = controller.GetTerrain();
                bool walkable = terrain.IsWalkable((int)clickPositionOnGrid.x, (int)clickPositionOnGrid.z);
                bool plantable = terrain.IsPlantable((int)clickPositionOnGrid.x, (int)clickPositionOnGrid.z);
                bool isTree = terrain.IsTree((int)clickPositionOnGrid.x, (int)clickPositionOnGrid.z);
                bool isShroom = terrain.IsShroom((int)clickPositionOnGrid.x, (int)clickPositionOnGrid.z);

                // -------------------------------------
                if (leftBtn) // If LEFT Mouse Button was Clicked
                {
                    // If clicked on walkable area then go directyl to it
                    if (walkable)
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.Walk_To));
                    }
                    // If clicked on unwalkable area, maybe it can be reached nearby?
                    else
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.Walk_To, 2)); // Constructing new action with flas = 2 which means Walk to Nearby area
                    }
                    
                }
                // -------------------------------------

                // -------------------------------------
                if (rightBtn) // If RIGHT Mouse Button was Clicked
                {
                    // If BallonPad is overthere
                    if (hit.collider.gameObject.CompareTag("BalloonPad"))
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.Open_Shop));
                    }
                    // If clicked on empty grass
                    else if (plantable)
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.Plant_Tree));
                    }
                    // If clicked on a tree
                    else if (isTree)
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.Chop_Tree));
                    }
                    // If clicked on a shroom
                    else if (isShroom)
                    {
                        AddToQue(new Action(clickPositionOnGrid, ActionType.PickUpShoorm));
                    }
                }
                // -------------------------------------


            }
            else
            {
                selector.SetActive(false); // Remove selector tool from world
            }
        }
        #endregion
        // -------------------------------------

        // -------------------------------------
        UpdateQue();
        PerformCurrentAction();
        // -------------------------------------

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
        // -------------------------------------

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
                    Current_Action.Set_Origin(GetPlayerPosition_Adjusted());
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
                case ActionType.PickUpShoorm:

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
            Current_Action.Set_Origin(GetPlayerPosition_Adjusted());
            Current_Action.active = false; // after we walked to this action, we must begin performing it, but it is inactive anymore, since we are not chopping it yet
            Action_Que.Clear();
        }
        else // if action was encountered for the first time, walk to it firstly
        {
            Vector3 plr_pos = GetPlayerPosition_Adjusted();
            if (controller.GetTerrain().IsAdjacent((int)onTop.dst_Pos.x, (int)onTop.dst_Pos.z, (int)plr_pos.x, (int)plr_pos.z)) // If we are nearby the action destination, just prepare it for execution
            {
                QueChanged = true;
                onTop.active = true;
            }
            else // If we are not nearby the action destination, let's walk to it first
            {
                Current_Action = new Action(onTop.dst_Pos, ActionType.Walk_To, 2);
                Current_Action.Set_Origin(GetPlayerPosition_Adjusted());
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
                    case ActionType.PickUpShoorm:
                        Debug.Log("Shroom PIKED");
                        Pick_Shoorm(Current_Action.dst_Pos);
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

        GameObject treeObj = controller.GetTerrain().Get_Vegetation_Object_From_Grid((int)pos.x, (int)pos.z);
        tree treescript = treeObj.GetComponent<tree>();
        treescript.Perform_Chop();
        Current_Action.done = true;
    }

    private void PerformAction_plant_tree_at(Vector3 pos)
    {
        controller.GetTerrain().Spawn_Tree_At((int)pos.x, (int)pos.z);
        Current_Action.done = true;
    }

    private void Pick_Shoorm(Vector3 pos)
    {
        
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

    /// <summary>
    /// Get palyer coordianes adjusted to Worl Grid coordinates (e.g.: x = 1.37 equals x = 1)
    /// </summary>
    private Vector3 GetPlayerPosition_Adjusted()
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
