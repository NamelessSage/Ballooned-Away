using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ = 0;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        cameraZ = transform.position.z;
        camera = GetComponent<Camera>();
        camera.fieldOfView = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            // Vector3 delta = target.transform.position - camera.ViewportToWorldPoint(new Vector3(0, cameraZ+2, 0));
            Vector3 destination = target.transform.position + new Vector3(-4, 5, -4);

            // transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            transform.position = destination;
        }
    }
}
