using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region DONT EDIT UNLESS YOU KNOW WHAT UR DOING. CHANIGN THIS INCORECTLY WILL MAKE WHOLE INVENTORY AND SHOP AT STUFF TO BREAK
[System.Serializable]
public class AcquirableAsset
{
    public int ID;// { get; }
    public string name;// { get; }

    public AcquirableAsset() { }
    public AcquirableAsset(int id, string n)
    {
        ID = id;
        name = n;
    }
}

/// <summary>
/// Defined data about resource(such as wood, stone, crystal...) available for player to acquire throughout the game that is used as score system or currency
/// </summary>
[System.Serializable]
public class ResourceUnit : AcquirableAsset
{
    public ResourceUnit() : base() { }
    public ResourceUnit(int id, string name) : base(id, name)
    {

    }
}

/// <summary>
/// Defined data about Item such as Blueprint, Sampling, Rune that player is able to keep in one of his inventory slots
/// </summary>
[System.Serializable]
public class GameItem : AcquirableAsset
{
    public GameObject obj { get; }

    public GameItem() : base() { }
    public GameItem(int id, string name, GameObject obj) : base(id, name)
    {
        this.obj = obj;
    }
}


public static class GlobalResourcesData
{
    public static List<ResourceUnit> AVAIALABE_Resources;


    public static void FeedResData(List<ResourceUnit> list)
    {
        AVAIALABE_Resources = list;
    }

    public static ResourceUnit GetResourceByName(string name)
    {
        foreach (ResourceUnit i in AVAIALABE_Resources)
        {
            if (i.name.Equals(name))
            {
                return i;
            }
        }

        return null;
    }
}

public static class GlobalItemsData
{
    public static List<GameItem> AVAIALABE_GameItems;


    public static void FeedItemData(List<GameItem> list)
    {
        AVAIALABE_GameItems = list;
    }

    public static GameItem GetItemByName(string name)
    {
        foreach (GameItem i in AVAIALABE_GameItems)
        {
            if (i.name.Equals(name))
            {
                return i;
            }
        }

        return null;
    }
}



/// <summary>
/// Global class that holds all infomation about possible game Acquirable Assets by the player (Resources, items, wood, blueprints....)
/// </summary>
public class AcquirableAssetsData : MonoBehaviour
{
    public List<ResourceUnit> AVAIALABE_Resources = new List<ResourceUnit>();
    public List<GameItem> AVAIALABE_GameItems = new List<GameItem>();

    public GameObject[] buildings;

    void Start()
    {
        MakeRes();
        MakeItems();

        GlobalResourcesData.FeedResData(AVAIALABE_Resources);
        GlobalItemsData.FeedItemData(AVAIALABE_GameItems);

        AVAIALABE_Resources = null;
        AVAIALABE_GameItems = null;
    }
    #endregion


    // Hi! I am a notice!
    // All the items you want to add to game should be included over here, bellow me
    // The region above me is the core for game inventory, shopping and other suff

    private void MakeRes()
    {
        AVAIALABE_Resources.Add(new ResourceUnit(1, "Wood"));
        AVAIALABE_Resources.Add(new ResourceUnit(2, "Stone"));
        AVAIALABE_Resources.Add(new ResourceUnit(3, "Iron"));
        AVAIALABE_Resources.Add(new ResourceUnit(4, "Food"));
        AVAIALABE_Resources.Add(new ResourceUnit(5, "Planks"));

        //AVAIALABE_Resources.Add(new ResourceUnit(id -  from 1 to ..., name - string));
    }

    private void MakeItems()
    {
        AVAIALABE_GameItems.Add(new GameItem(1, "Axe", null));
        AVAIALABE_GameItems.Add(new GameItem(2, "Blueprint: Lumbermill", buildings[0]));
        AVAIALABE_GameItems.Add(new GameItem(3, "Blueprint: Blacksmith", buildings[1]));
        AVAIALABE_GameItems.Add(new GameItem(4, "Blueprint: Campfire", null));
        AVAIALABE_GameItems.Add(new GameItem(5, "Apple", null));
        AVAIALABE_GameItems.Add(new GameItem(6, "Rock", null));
    }
}

