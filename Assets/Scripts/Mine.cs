using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = GetMousePos();
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit))
        {
            if (hit.collider.tag == "Tree" && Input.GetMouseButtonDown(1))
            {
                tree treescript = hit.collider.gameObject.GetComponent<tree>();
                treescript.treeHealth--;
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
