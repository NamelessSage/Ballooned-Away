using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DayNightCycle : MonoBehaviour
{
    [Range(0,30)]
    public float sunSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.rotation.eulerAngles.x);
        if (transform.rotation.eulerAngles.x>=190 && transform.rotation.eulerAngles.x<=350)
        {
            transform.RotateAround(Vector3.zero, Vector3.right,160);
            transform.LookAt(Vector3.zero);
        }
        transform.RotateAround(Vector3.zero, Vector3.right, sunSpeed*Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }
}
