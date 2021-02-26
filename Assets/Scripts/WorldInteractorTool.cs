using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInteractorTool : MonoBehaviour
{
    public GameObject GameController;
    public GameObject selector;
    
    private GameControllerScript controller;


    void Start()
    {
        controller = GameController.GetComponent<GameControllerScript>();

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
    }

    private void movePlayer(Vector3 newPos)
    {
        // MUST BE CHANGED
        var playerObj = controller.playerObj;
        // Vector3 curPos = playerObj.transform.position;
        PathfindingService pathfinding = new PathfindingService();
        pathfinding.GetAstarPath(controller.playerObj, newPos, controller.GetTerrain());

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
