using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private class Item
    {
        public string Name;
        public int Price;

        public Item(string name, int price)
        {
            this.Name = name;
            this.Price = price;
        }
    }
 
    public GameObject ShopPanel;
    
    public GameObject Events_Obj;
    private EventSystem Events;
    
    private Item [] ShopItems = new Item[3];
    public Text [] NameLabels;
    public Text [] PriceLabels;
    void Start()
    {
        Events = Events_Obj.GetComponent<EventSystem>();
        ShopItems[0] = new Item("Trash1", 1);
        ShopItems[1] = new Item("Trash2", 2);
        ShopItems[2] = new Item("Trash3", 1);
            
        OpenShop();

    }
    
    /// <summary>
    /// Open shop's UI
    /// </summary>
    public void OpenShop()
    {
        for (int i = 0; i < ShopItems.Length; i++)
        {
            NameLabels[i].text = ShopItems[i].Name;
            PriceLabels[i].text = ShopItems[i].Price.ToString();
        }
        
        if (ShopPanel != null)
        {
            ShopPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Close shop's window
    /// </summary>
    public void CloseShop()
    {
        if (ShopPanel != null && ShopPanel.activeSelf.Equals(true))
        {
            ShopPanel.SetActive(false);
        }
    }
    
    public void BuyOfferOne()
    {
        Debug.Log(ShopItems[0]);
    }

    public void BuyOfferTwo()
    {
        Debug.Log(ShopItems[1]);
        
    }
    public void BuyOfferThree()
    {
        Debug.Log(ShopItems[2]);
        
    }
}


