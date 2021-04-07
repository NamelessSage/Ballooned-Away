using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TradeType
{
    Resource,
    Blueprint,
    Item
}

public class Transaction
{
    public bool success;
    
    public string pName;
    public int pN;

    public string iName;
    public int iN;

    public Transaction() { }
    public Transaction(bool succ)
    {
        success = succ;
    }

    public Transaction(bool succ, string pName, string iName, int pN, int iN)
    {
        success = succ;
        this.pName = pName;
        this.iName = iName;
        this.pN = pN;
        this.iN = iN;
    }
}

[System.Serializable]
public class Trade
{
    public string name;
    public int amount;
    public TradeType type;

    public string priceName;
    public int priceAmount;

    public Trade() { }

    public Trade(string name, int amount, string priceName, int priceAmount, TradeType type)
    {
        this.name = name;
        this.amount = amount;
        this.priceName = priceName;
        this.priceAmount = priceAmount;
        this.type = type;
    }
}

public class BalloonPad : MonoBehaviour
{
    /// <summary>
    /// Holds infromation about available trades by the balloon, each type of trade is stored in separate array
    /// </summary>
    private class TradeLists
    {
        public Trade[] BluePrints = { new Trade("Blueprint: Lumbermill", 1, "Wood", 100, TradeType.Blueprint),
                                      new Trade("Blueprint: Blacksmith", 1, "Stone", 100, TradeType.Blueprint) };

        public Trade[] Resources =  { new Trade("Wood", 20, "Stone", 10, TradeType.Resource),
                                      new Trade("Stone", 10, "Wood", 20, TradeType.Resource),
                                      new Trade("Iron", 1,  "Stone", 10, TradeType.Resource) };

        public Trade[] Items =      { new Trade("Axe", 1,   "Wood", 50, TradeType.Item),
                                      new Trade("Apple", 1,  "Wood", 1, TradeType.Item),
                                      new Trade("Rock", 3, "Stone", 10, TradeType.Item) };

        public TradeLists() { }
    }

    private TradeLists Trades = new TradeLists();

    //-------------------------------------------------------
    //-------------------------------------------------------

    private PlayerGuiController player_UI;
    private Inventory PlrInventory;

    //-------------------------------------------------------

    private int WaitTime = 5;
    private float PassedTime = 0;

    private bool arrived = false;

    void Start()
    {
        PassedTime = WaitTime;
    }
    
    void Update()
    {
        PassedTime = PassedTime - Time.deltaTime;
        if (PassedTime <= 0)
        {
            if (arrived == false)
            {
                arrived = true;
                player_UI.BalloonArivedEvent(this, CreateBalloonOffers());
                transform.GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                arrived = false;
                player_UI.BallonRejected();
                Reject();
                
            }

            PassedTime = WaitTime;

        }
    }

    private List<Trade> CreateBalloonOffers()
    {
        List<Trade> newTradeList = new List<Trade>();

        int random = Random.Range(0, Trades.Resources.Length);
        newTradeList.Add(Trades.Resources[random]);

        random = Random.Range(0, Trades.BluePrints.Length);
        newTradeList.Add(Trades.BluePrints[random]);

        random = Random.Range(0, Trades.Items.Length);
        newTradeList.Add(Trades.Items[random]);

        return newTradeList;
    }



    public void Reject()
    {
        Debug.Log(this.name + " my session was closed. Im forced to leave and will return LATER");
        transform.GetChild(3).gameObject.SetActive(false);

    }


    public Transaction PerformTrade(Trade i)
    {
        Debug.Log(this.name + " trading: " + i.name + " for " + i.priceName + " x" + i.priceAmount);

        if (PlrInventory.Resources_CheckIfEnoughResource(i.priceName, i.priceAmount))
        {
            int newAmount = PlrInventory.Resources_ConsumeResource(i.priceName, i.priceAmount);
            if (i.type == TradeType.Resource)
            {
                PlrInventory.Resources_AddToResources(i.name, i.amount);   
            }
            else
            {
                PlrInventory.Inventory_AddToInventory(i.name, i.amount);
            }
            

            return new Transaction(true, i.priceName, i.name, newAmount, i.amount);
        }
        else
        {
            Debug.Log("Not Enough Resources");
            return new Transaction(false);
        }

    }

    public void SetPlayerUI(PlayerGuiController gui)
    {
        player_UI = gui;
    }

    public void SetPlayerInventory(Inventory inv)
    {
        PlrInventory = inv;
    }



}
