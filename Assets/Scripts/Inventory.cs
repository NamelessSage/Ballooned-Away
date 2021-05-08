using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region InventoryClasses
    /// <summary>
    /// Resource and it's amount owned by the player
    /// </summary>
    [System.Serializable]
    private class OwnedResource
    {
        public ResourceUnit resourse;
        public int amount;

        public OwnedResource() { }
        public OwnedResource(ResourceUnit res, int amount)
        {
            resourse = res;
            this.amount = amount;
        }
    }

    /// <summary>
    /// Game Item and it's amount owned by the player
    /// </summary>
    [System.Serializable]
    private class OwnedGameItem
    {
        public GameItem item;
        public int amount;

        public OwnedGameItem() { }
        public OwnedGameItem(GameItem item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }
    #endregion

    public GameControllerScript controller;
    public PlayerGuiController guiObj;

    [SerializeField]
    private List<OwnedResource> Player_Resources;
    [SerializeField]
    private List<OwnedGameItem> Player_Inventory;// = new List<OwnedGameItem>(3);

    private PlayerGuiController gui;

    void Start()
    {
        gui = guiObj.GetComponent<PlayerGuiController>();
    }


    #region Resources
    /// <summary>
    /// Checks if player has the same amount or more of the given resource
    /// </summary>
    /// <param name="name"> name of resource to look up </param>
    /// <param name="amount"> amout of resource to compare </param>
    /// <returns> true if player has equal amount or more of the given resource </returns>
    public bool Resources_CheckIfEnoughResource(string name, int amount)
    {
        int index = Resources_FindIndexOf_byName(name);
        if (index >= 0)
        {
            OwnedResource res = Player_Resources[index];

            if (res.amount >= amount) return true;
        }

        return false;
    }

    /// <summary>
    /// Subtracts amout of given resource (and removes it form player if new amount is 0 (and less)) by specified number
    /// </summary>
    /// <param name="name"> resource to decrease </param>
    /// <param name="amount"> number to subtract </param>
    /// <returns> new amount of the resource </returns>
    public int Resources_ConsumeResource(string name, int amount)
    {
        int index = Resources_FindIndexOf_byName(name);
        if (index >= 0)
        {
            OwnedResource res = Player_Resources[index];

            res.amount -= amount;

            if (res.amount <= 0)
            {
                Player_Resources.RemoveAt(index);
                gui.UpdateResourcesTrackers(name);
                return 0;
            }
            else
            {
                gui.UpdateResourcesTrackers(name);
                return res.amount;
            }

            
        }

        Debug.Log("Such " + name + " does not exist in resources");
        return -1;
    }

    /// <summary>
    /// Return amount of resource if player has it in inventory
    /// </summary>
    /// <param name="name"> resource to lookup </param>
    /// <returns> -1 if resource does not exist, otherwise returns its amount </returns>
    public int Resources_GetAmount(string name)
    {
        int index = Resources_FindIndexOf_byName(name);
        if (index >= 0)
        {
            OwnedResource res = Player_Resources[index];
            return res.amount;
        }

        Debug.Log("Such " + name + " does not exist in resources");
        return -1;
    }

    /// <summary>
    /// Increase or give resource to the player with specified number (if player doesnt have that resource, add that resource (and its amount) to his inventory)
    /// </summary>
    /// <param name="name"> name of the resource to augment </param>
    /// <param name="amount"> amount to add or increase (leave blank for default 1) </param>
    /// <returns> new amount of the resource </returns>
    public int Resources_AddToResources(string name, int amount = 1)
    {
        int index = Resources_FindIndexOf_byName(name);
        if (index >= 0)
        {
            OwnedResource res = Player_Resources[index];
            res.amount += amount;
            gui.UpdateResourcesTrackers(name);
            return res.amount;
        }
        else
        {
            ResourceUnit g = GlobalResourcesData.GetResourceByName(name);
            
            if (g != null)
            {
                Debug.Log("Adding new resource " + name);
                Player_Resources.Add(new OwnedResource(g, amount));
                gui.UpdateResourcesTrackers(name);
                return amount;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds index of the given resource in inventory (if present)
    /// </summary>
    /// <param name="name"> resource to look for </param>
    /// <returns> -1 if the resource is not present, index otherwise </returns>
    public int Resources_FindIndexOf_byName(string name)
    {
        int index = 0;

        foreach (OwnedResource ow in Player_Resources)
        {
            if (ow.resourse.name.Equals(name)) return index;
            index++;
        }

        return -1;
    }

    #endregion

    #region Inventory
    /// <summary>
    /// Checks if player has a given item in his backpack
    /// </summary>
    /// <param name="name"> item name </param>
    /// <returns> true if contains item in backpack </returns>
    public bool Inventory_CheckIfHasItem(string name)
    {
        foreach (OwnedGameItem i in Player_Inventory)
        {
            if (i.item.name.Equals(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Adds new item or augments amount of item if already has in inventory, by default
    /// </summary>
    /// <param name="name"> item name to add or augment </param>
    /// <param name="amount"> amount of item to add (leave blank for default value = 1) </param>
    /// <returns> new amount <returns>
    public int Inventory_AddToInventory(string name, int amount = 1)
    {
        foreach (OwnedGameItem i in Player_Inventory)
        {
            if (i.item.name.Equals(name))
            {
                i.amount += amount;
                gui.Notify_success();
                return i.amount;
            }
        }

        GameItem g = GlobalItemsData.GetItemByName(name);
        if (g != null)
        {
            Debug.Log("Adding new item " + name);
            Player_Inventory.Add(new OwnedGameItem(g, amount));
            gui.Notify_success();
            return amount;
        }

        Debug.Log("Such " + name + " does not exist in inventory");
        return -1;
    }

    /// <summary>
    /// Return only the given item reference if contains
    /// </summary>
    /// <param name="name"> item to return </param>
    /// <returns> null if does not contain; the Item reference if does </returns>
    public GameItem Inventory_GetItem(string name)
    {
        GameItem item = null;
        foreach (OwnedGameItem i in Player_Inventory)
        {
            if (i.item.name.Equals(name))
            {
                item = i.item;
                break;
            }
        }

        return item;
    }

    /// <summary>
    /// Return only the given item amount if contains
    /// </summary>
    /// <param name="name"> item to lookup </param>
    /// <returns> -1 if does not contain; item amount if does </returns>
    public int Inventory_GetAmountOfItem(string name)
    {
        int am = -1;
        foreach (OwnedGameItem i in Player_Inventory)
        {
            if (i.item.name.Equals(name))
            {
                am = i.amount;
                break;
            }
        }

        return am;
    }

    /// <summary>
    /// Subtracts amout of given item (and removes it form player if new amount is 0 (and less)) by specified number
    /// </summary>
    /// <param name="name"> item to decrease </param>
    /// <param name="amount"> number to subtract </param>
    /// <returns> new amount of the item </returns>
    public int Inventory_ConsumeItem(string name, int amount)
    {

        int index = Inventory_FindIndexOf_byName(name);
        if (index >= 0)
        {
            OwnedGameItem res = Player_Inventory[index];

            res.amount -= amount;

            if (res.amount <= 0)
            {
                Player_Inventory.RemoveAt(index);
                return 0;
            }
            else
            {
                return res.amount;
            }
        }

        Debug.Log("Such " + name + " does not exist in inentory");
        return -1;
    }

    private int Inventory_FindIndexOf_byName(string name)
    {
        int index = 0;

        foreach (OwnedGameItem ow in Player_Inventory)
        {
            if (ow.item.name.Equals(name)) return index;
            index++;
        }

        return -1;
    }

    public InventorySlotInfo[] GetArrayOfItemsInInventory()
    {
        int len = Player_Inventory.Count;
        InventorySlotInfo[] array = new InventorySlotInfo[len];

        for (int i = 0; i < len; i++)
        {
            array[i] = new InventorySlotInfo(Player_Inventory[i].item.name, Player_Inventory[i].amount);
        }

        return array;
    }

    public void DrawItem(string name)
    {
        int indx = Inventory_FindIndexOf_byName(name);
        if (indx > -1 && Player_Inventory[indx].item.obj != null)
        {
            controller.PlayerSpawnBuilding(name);
        }
    }

    #endregion
}
