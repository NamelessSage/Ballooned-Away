using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ = 0;
    private Camera player_camera;

    private float Radius = 4.2f;
    public float circleAngle = 0;
    private int angle = 60;

    private Vector3 mainVec = new Vector3(0, 5, -4.2f);
    private Vector3 rotate = new Vector3(60, 0, 0);
    private Vector3 oldPos = Vector3.zero;



    void Start()
    {
        cameraZ = transform.position.z;
        player_camera = GetComponent<Camera>();
        player_camera.fieldOfView = 60;
    }

    void Update()
    {

    // Update is called once per frame
        Vector3 thisPos = Input.mousePosition;
        int mult = (thisPos.x < oldPos.x) ? -1 : 1;

        Vector2 scroll = Input.mouseScrollDelta;
        bool midClick = Input.GetMouseButton(2);
    
        if (scroll.y > 0f || scroll.y < 0f)
        {
            int O = (int)scroll.y;
            angle -= O*2;
            if (angle > 60) angle = 60;
            if (angle < 30) angle = 30;

            rotate = new Vector3(angle, rotate.y, 0);
        }

        if (midClick)
        {
            
            float newAngle = (mult * ((Mathf.Abs(thisPos.x - oldPos.x)))) * 0.1f;
            circleAngle += newAngle;

            float newX = Radius * Mathf.Sin(circleAngle * Mathf.PI/180);
            float newY = Radius * Mathf.Cos(circleAngle * Mathf.PI/180);

            mainVec = new Vector3(-newX, 5, -newY);
            rotate = new Vector3(rotate.x, circleAngle, 0);
        }

        if (target)
        {
            Vector3 destination = target.transform.position + mainVec;
            transform.position = destination;
            transform.eulerAngles = rotate;
        }

        oldPos = thisPos;
    }
}

