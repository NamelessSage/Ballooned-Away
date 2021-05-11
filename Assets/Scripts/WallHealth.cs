using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallHealth : MonoBehaviour
{

    public Slider slider;
    public int Health;
    GameObject thisObject;
    
    private void Start()
    {
        setmaxHealt(Health);
        thisObject = transform.gameObject;
    }

    private void setmaxHealt(int health)
    {
        slider.value = health;
        slider.maxValue = health;
    }
    private void setHealt(int healt)
    {
        slider.value = healt;
    }
    
    private void takedamage(int damage)
    {
        Health -= damage;
        setHealt(Health);
        if (Health < 0)
        {
            DestroyObj();
        }
    }

    private void DestroyObj()
    {
        Destroy(thisObject);
    }
    
}
