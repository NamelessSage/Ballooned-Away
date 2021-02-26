using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


public class WorldInteractorTool : MonoBehaviour
{

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
            Vector3 position = GetMousePos();
            if (position.x >= 0)
            {
                GameObject target = controller.GetTerrainObjectFromPosition(position);
                if (target != null)
                {
                    // Debug.Log(position);
                    Vector3 targetPos = target.transform.position;
                    Vector3 newSelectroPos = new Vector3(targetPos.x,
                                                         targetPos.y + (target.transform.localScale.y / 2) + 0.005f,
                                                         targetPos.z);
                    selector.SetActive(true);
                    selector.transform.position = newSelectroPos;

                    movePlayer(targetPos);
                }
            }
            else
            {
                selector.SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Vector3 fwd = GetMousePos();
            Vector3 player = controller.playerObj.transform.position;
            float dist = Vector3.Distance(player, fwd);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit))
            {
                if (hit.collider.CompareTag("Tree"))
                {
                    if (dist <= 1.3f)
                    {
                        tree treescript = hit.collider.gameObject.GetComponent<tree>();
                        Text script = hit.collider.gameObject.GetComponentInChildren<Text>();
                        treescript.change(script);
                    }
                }
            }
        }
        
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

    private void movePlayer(Vector3 newPos)
    {

        PathfindingService pathfinding = new PathfindingService();

        Vector3 fromPos = controller.adjustCords(player.transform.position);

        List<Node> newPath = pathfinding.GetAstarPath(fromPos, newPos, controller.GetTerrain());
        
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

    private void GetNextNode()
    {
        if (path!=null)
        {
            if (curNode >= path.Count-1)
            {
                pathFound = false;
                curNode = 0;
                path.Clear();
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
}
