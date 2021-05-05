using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{ 
    
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
