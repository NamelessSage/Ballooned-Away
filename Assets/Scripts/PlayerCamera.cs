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

    private Vector3 mainVec = new Vector3(-3, 5, -3);
    private Vector3 rotate = new Vector3(60, 45, 0);

    private int angle = 60;
    void Start()
    {
        cameraZ = transform.position.z;
        player_camera = GetComponent<Camera>();
        player_camera.fieldOfView = 60;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 scroll = Input.mouseScrollDelta;
        if (scroll.y > 0f || scroll.y < 0f)
        {
            int O = (int)scroll.y;
            angle -= O*2;
            if (angle > 60) angle = 60;
            if (angle < 30) angle = 30;

            rotate = new Vector3(angle, 45, 0);
        }

        if (target)
        {
            Vector3 destination = target.transform.position + mainVec;
            transform.position = destination;
            transform.eulerAngles = rotate;
        }


    }
}


/*
 * GLOBAL -     private float height = 5;
 *  Vector2 scroll = Input.mouseScrollDelta;
        if (scroll.y > 0f || scroll.y < 0f)
        {
            int O = (int)scroll.y;
            angle += O;
            if (angle > 90) angle = 90;
            if (angle < 20) angle = 20;

            if (scroll.y > 0) height += 0.1f;
            if (scroll.y < 0) height -= 0.1f;

            if (angle >= 60) height = 5;

            if (height > 5) height = 5;
            if (height < 3.5f) height = 3.5f;

            Debug.Log(angle);

            if (angle == 60)
            {
                mainVec = new Vector3(-3, 5, -3);
                rotate = new Vector3(60, 45, 0);
            }
            
            if (scroll.y > 0 && angle < 90)
            {
                if (angle < 60)
                {
                    mainVec = new Vector3(mainVec.x - (0.1f * O), height + (0.1f), mainVec.z - (0.1f * O));
                    rotate = new Vector3(angle, 45, 0);
                }
                else
                {
                    mainVec = new Vector3(mainVec.x + (0.1f * O), height, mainVec.z + (0.1f * O));
                    rotate = new Vector3(angle, 45, 0);
                }
            }
            else if (scroll.y < 0 && angle > 20)
            {
                if (angle > 60)
                {
                    mainVec = new Vector3(mainVec.x + (0.1f * O), height, mainVec.z + (0.1f * O));
                    rotate = new Vector3(angle, 45, 0);
                }
                else
                {
                    mainVec = new Vector3(mainVec.x - (0.1f * O), height - 0.1f, mainVec.z - (0.1f * O));
                    rotate = new Vector3(angle, 45, 0);
                }
            }   
            else if (angle == 90)
            {
                mainVec = new Vector3(0, 5, 0);
                rotate = new Vector3(angle, 45, 0);
            }
            else if (angle == 20)
            {
                mainVec = new Vector3(-0.9f, 3.5f, -0.9f);
                rotate = new Vector3(angle, 45, 0);
            }
        }
 */
