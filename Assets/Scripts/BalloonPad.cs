using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalloonPad : MonoBehaviour
{
    public string [] ItemsNames;
    public int [] ItemsPrices;

    private string[] CurrentItemsOnSaleNames = new string[3];
    private int[] CurrentItemsOnSalePrice = new int[3];

    private int WaitTime = 5;
    private float PassedTime = 0;
    void Start()
    {
        CreateBalloonOffers();
        PassedTime = WaitTime;
    }
    
    void Update()
    {
        PassedTime = PassedTime - Time.deltaTime;
        if (PassedTime <= 0)
        {
            CreateBalloonOffers();
            PassedTime = WaitTime;
        }
    }

    private void CreateBalloonOffers()
    {
        for (int i = 0; i < 3; i++)
        {
            int random = Random.Range(0, ItemsNames.Length);
            CurrentItemsOnSaleNames[i] = ItemsNames[random];
            CurrentItemsOnSalePrice[i] = ItemsPrices[random];
        }
    }

    public string[] GetItemsOnSale1()
    {
        return CurrentItemsOnSaleNames;
    } 
    public int[] GetItemsOnSale2()
    {
        return CurrentItemsOnSalePrice;
    }

}
