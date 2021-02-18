using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = GetMousePos();
            if (position.x >= 0)
            {
                GameObject target = controller.getTerrainObjectFromPosition(position);
                if (target != null)
                {
                    Debug.Log(position);
                    Vector3 targetPos = target.transform.position;
                    Vector3 newSelectroPos = new Vector3(targetPos.x,
                                                         targetPos.y + (target.transform.localScale.y / 2) + 0.005f,
                                                         targetPos.z);
                    selector.SetActive(true);
                    selector.transform.position = newSelectroPos;
                }
            }
            else
            {
                selector.SetActive(false);
            }
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
