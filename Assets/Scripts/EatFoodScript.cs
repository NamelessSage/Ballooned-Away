using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFoodScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Player").GetComponent<Skills>().heal(20);
        Destroy(gameObject);
    }

}
