using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    public GameObject chan;
    // Start is called before the first frame update
    void LateUpdate()
    {
        transform.LookAt(transform.position - new Vector3(-4, 5, -4));
    }
}
