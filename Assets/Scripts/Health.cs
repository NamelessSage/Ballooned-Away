using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider slider;

    public void SetHealth(int temp)
    {
        slider.value = temp;
    }
    public void SetMaxHealth(int temp)
    {
        slider.maxValue = temp;
        slider.value = temp;
    }
}
