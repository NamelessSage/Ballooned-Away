﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Sawmill : MonoBehaviour
{
    public GameObject ControlerObj;
    public GameObject DepsitColliderObj;
    public GameObject WithdrawObj;
    private GameControllerScript Controller;
    private int TotalAmountOfWood;
    private int TotalAmountOfPlanks;
    public int WaitTime;
    public int ConversionRatio;
    private bool CuttingPlanks = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Controller = ControlerObj.GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TotalAmountOfWood > 0)
        {
            if (CuttingPlanks == false)
            {
                CuttingPlanks = true;
                StartCoroutine(ConvertWoodToPlanks(WaitTime));
            }
        }
    }

    public void AddToQueue(int aow)
    {
        TotalAmountOfWood += aow;
    }

    public int TakePlanks(int amountToTake)
    {
        int availableAmount = TotalAmountOfPlanks;
        if (TotalAmountOfPlanks < amountToTake)
        {
            TotalAmountOfPlanks = 0;
            return availableAmount;
        }
        else
        {
            availableAmount = TotalAmountOfPlanks - amountToTake;
            return amountToTake;
        }
    }
    
    private IEnumerator ConvertWoodToPlanks(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TotalAmountOfPlanks += ConversionRatio;
        TotalAmountOfWood--;
        CuttingPlanks = false;
    }

}