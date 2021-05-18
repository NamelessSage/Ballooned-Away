using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotInfo
{
    public string name;
    public int am;
    public Sprite icon;

    public InventorySlotInfo() { }

    public InventorySlotInfo(string n, int a, Sprite ico)
    {
        name = n; am = a;
        icon = ico;
    }
}

public class PlayerGuiController : MonoBehaviour
{
    #region PRIVATE CLASSES AND LISTS
    private int maxCapacity = 100; // WILL BE REMOVED LATER

    // =====================================
    // ==== PRIVATE CLASSES AND LISTS ======
    // =====================================

    [System.Serializable]
    private class Tracker
    {
        public string name;
        public int load = 0;
        public int pos = -1;

        public Tracker() { }

        public Tracker(string n)
        {
            name = n;
        }
    }

    [System.Serializable]
    private class BackpackSlot
    {
        public string name;
        public int amount;
        public int pos = -1;
        public Sprite icon;

        public BackpackSlot() { }

        public BackpackSlot(string n, int a, Sprite ico = null)
        {
            name = n;
            amount = a;
            icon = GlobalItemsData.GetItemByName(n).icon;
        }
    }

    [System.Serializable]
    private class CurrentBallon
    {
        public List<Trade> TradeList;
        public BalloonPad Balloon;

        public CurrentBallon(List<Trade> list, BalloonPad pad)
        {
            TradeList = list;
            Balloon = pad;
        }
    }

    // =====================================
    // =====================================


    [SerializeField] private List<Tracker> TrackersList;
    [SerializeField] private BackpackSlot[] BackpackList;

    #endregion

    // Templates prefabs
    public GameObject trackerTemplate;
    public GameObject slotTemplate;
    // --------------------------------

    // Objects references
    public GameObject controllerObj;
    // ---------------------------------

    // UI panels references
    public GameObject ballonShopUI;
    public GameObject BrokenBallonUI;
    public GameObject backpackUI;
    public GameObject inventoryUI;
    public GameObject trackersPanel;
    public GameObject balloonShopMainSlotsPanel;
    public GameObject inventorySlotsPanel;
    public GameObject skillTreeUi;
    // ---------------------------------

    // Script references
    private GameControllerScript controller;
    private Inventory playerINV;
    // ---------------------------------

    [SerializeField] private CurrentBallon Current_Balloon;

    private bool ballonShopActive = false;

    private bool inventoryOpen = false;

    private bool skillTreeActive = false;

    private int curenltySelectedToolbeltSlot = -1;


    void Start()
    {
        BackpackList = new BackpackSlot[4];
        for (int i = 0; i < 4; i++)
        {
            BackpackList[i] = null;
        }

        controller = controllerObj.GetComponent<GameControllerScript>();
        playerINV = controller.GetComponent<Inventory>();
        skillTreeUi.SetActive(false);


        CloseShop();
        CloseInv();
        CloseBrokenUI();

        UpdateBackpackUi();
    }

    void Update()
    {
        DoInputCheck();
        OutlineSelectedItem();
    }

    #region Trackers Methods

    /// <summary>
    /// Updates all trackers or looks up a specific one. If it does not exist, add him
    /// </summary>
    /// <param name="name"> leave blank if updating everything OR specify new or specific resource name </param>
    public void UpdateResourcesTrackers(string name = "")
    {
        int i = FindTracker(name);
        if (i >= 0 || name.Equals(""))
        {
            for(int j=0; j < TrackersList.Count; j++)
            {
                GameObject theTracker = trackersPanel.transform.GetChild(j).gameObject;
                Tracker t = TrackersList[j];
                int a = playerINV.Resources_GetAmount(t.name);

                if (a == -1) a = 0;

                theTracker.transform.GetChild(2).GetComponent<Text>().text = ("" + a);

                float howMuch = (float)a / maxCapacity;
                if (howMuch >= 0.33f) theTracker.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                else theTracker.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.078f);
                if (howMuch >= 0.66f) theTracker.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                else theTracker.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0.078f);
                if (howMuch >= 0.99f) theTracker.transform.GetChild(1).transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                else theTracker.transform.GetChild(1).transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0.078f);
            }
        }
        else
        {
            AddTracker(name);
            UpdateResourcesTrackers(name);
        }
    }

    private int FindTracker(string name)
    {
        int i = 0;
        foreach (Tracker t in TrackersList)
        {
            if (t.name.Equals(name)) return i;

            i++;
        }

        return -1;
    }

    private void AddTracker(string name)
    {
        int  i = playerINV.Resources_FindIndexOf_byName(name);
        if (i >= 0)
        {
            TrackersList.Add(new Tracker(name));
            GameObject newTracker = Instantiate(trackerTemplate, trackersPanel.transform);
            newTracker.transform.GetChild(0).GetComponent<Text>().text = (name);
        }
    }
    #endregion

    #region Ballon Shop Methods
    public void BalloonArivedEvent(BalloonPad pad, List<Trade> newList)
    {
        Current_Balloon = new CurrentBallon(newList, pad);
        ballonShopActive = true;
        
    //    Debug.Log(this.name + " BallonArived?");
    }

    private void UpdateShop()
    {
        for (int i = 0; i < 3; i++)
        {
            string amount = "";
            //if (Current_Balloon.TradeList[i].type == TradeType.Resource)
           // {
                int theAmount = Current_Balloon.TradeList[i].amount;
                if (theAmount > 1)
                    amount = " x" + theAmount;
           // }

            balloonShopMainSlotsPanel.transform.GetChild(i).transform.GetChild(1).gameObject.GetComponent<Image>().sprite = Current_Balloon.TradeList[i].getIcon();
            balloonShopMainSlotsPanel.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Text>().text = Current_Balloon.TradeList[i].name + amount;
            string priceText = Current_Balloon.TradeList[i].priceName + " x" + Current_Balloon.TradeList[i].priceAmount;
            balloonShopMainSlotsPanel.transform.GetChild(i).transform.GetChild(2).gameObject.GetComponent<Text>().text = priceText;
        }
    }

    public void TradeSellectedOffer(int i)
    {
        if (ballonShopActive)
        {
            ballonShopActive = false;
            
            Transaction transaction = Current_Balloon.Balloon.PerformTrade(Current_Balloon.TradeList[i-1]);

            if (transaction.success == false)
            {
                ballonShopActive = true;
            }
            else
            {
                RejectBallon();

                UpdateResourcesTrackers();
                AddToBackpackList(transaction.iName);
            }

        }
    }

    public void ClearShop()
    {
        ballonShopActive = false;
        CloseShop();
        Current_Balloon = null;
    }

    public void RejectBallon()
    {
        if (Current_Balloon != null) Current_Balloon.Balloon.Reject();
        BallonRejected();
    }

    public void RepairBallon()
    {
        controller.AttemptRepair();
    }
    public void FORCERepairBallon()
    {
        controller.FORCEAttemptRepair();
    }


    public void BallonRejected()
    {
        ClearShop();
    }

    public void OpenBrokenPadUI()
    {
        DisplayBrokenPad();
    }

    public void OpenShop()
    {
        if (ballonShopActive)
        {
            DisplayShop();
            UpdateShop();
        }
        else
        {
           // Debug.Log(this.name + " sorry, ballon is not here yet :( ");
        }
    }

    public void CloseShop()
    {
        ballonShopUI.SetActive(false);
    }

    public void CloseBrokenUI()
    {
        BrokenBallonUI.SetActive(false);
    }

    private void DisplayShop()
    {
        ballonShopUI.SetActive(true);
    }

    private void DisplayBrokenPad()
    {
        BrokenBallonUI.SetActive(true);
    }

    #endregion

    #region Player Inventory

    private void OutlineSelectedItem()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject slt = backpackUI.transform.GetChild(i).gameObject;
            slt.transform.GetComponent<Image>().color = new Color(0, 0, 0, 0.588f);
            if (i == curenltySelectedToolbeltSlot) slt.transform.GetComponent<Image>().color = new Color(0.18f, 0.54f, 0.2f, 0.588f);
        }
    }

    private void DoInputCheck()
    {
        int num = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            num = 0;
            //DrawItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            num = 1;
            //DrawItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            num = 2;
            //DrawItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            num = 3;
            //DrawItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenInv();
            UpdateBackpackUi();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreeActive = !skillTreeActive;
            skillTreeUi.SetActive(skillTreeActive);
        }

        if (num > -1)
        {
            if (!EventSystem.current.IsPointerOverGameObject()) DrawItem(num);
            else
            {
                GameObject clickObj = RaycastMouse();
                if (clickObj.CompareTag("InventorySlotCollider"))
                {
                    string clickSlotName = clickObj.transform.parent.GetChild(1).gameObject.GetComponent<Text>().text;
                    AddToBackpackList(clickSlotName, num);
                }
            }
        }
    }

    private GameObject RaycastMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results[0].gameObject;
    }


    private void DrawItem(int i)
    {
        if (curenltySelectedToolbeltSlot == i)
        {
            curenltySelectedToolbeltSlot = -1;
            Notify_fail();
            controller.PlayerCancelSpawnBuilding();
        }
        else
        {
            curenltySelectedToolbeltSlot = i;
            if (BackpackList[i] != null)
            {
                string nam = BackpackList[i].name;
                playerINV.DrawItem(nam);

            }
        }
    }

    private void AddToBackpackList(string name, int slotNum = -1)
    {
        //Debug.LogWarning("was - " + name);
        int am = playerINV.Inventory_GetAmountOfItem(name);
        if (am >= 0)
        {
            bool was = false;

            if (slotNum == -1)
            {
                for (int i = 0; i < BackpackList.Length; i++)
                {
                    if (BackpackList[i] != null)
                    {

                        if (BackpackList[i].name.Equals(name))
                        {
                            BackpackList[i].amount = am;
                            was = true;
                            break;
                        }
                    }
                }
            }

            if (!was)
            {
                for (int i = 0; i < BackpackList.Length; i++)
                {
                    if ( slotNum > -1)
                    {
                        if (i == slotNum)
                        {
                            BackpackList[i] = new BackpackSlot(name, am);
                        }
                        else if (BackpackList[i] != null && BackpackList[i].name.Equals(name))
                        {
                            BackpackList[i] = null;
                        }
                    }
                    else
                    {
                        if (BackpackList[i] == null)
                        {
                            BackpackList[i] = new BackpackSlot(name, am);
                            break;
                        }
                    }
                }
            }

            UpdateBackpackUi();
        }
    }

    public void UpdateBackpackUi()
    {
        for (int i = 0; i < BackpackList.Length; i++)
        {
            GameObject a = backpackUI.transform.GetChild(i).gameObject;

            Image imgOnslot = a.transform.GetChild(0).gameObject.GetComponent<Image>();

            if (BackpackList[i] != null && playerINV.Inventory_CheckIfHasItem(BackpackList[i].name))
            {
                BackpackList[i].amount = playerINV.Inventory_GetAmountOfItem(BackpackList[i].name);
                imgOnslot.enabled = true;
                imgOnslot.sprite = BackpackList[i].icon;
                a.transform.GetChild(2).gameObject.GetComponent<Text>().text = "x" + BackpackList[i].amount;
            }
            else
            {
                BackpackList[i] = null;

                imgOnslot.sprite = null;
                imgOnslot.enabled = false;
                a.transform.GetChild(2).gameObject.GetComponent<Text>().text = "";
            }
        }

    }


    private void UpdateInventoryUi()
    {
        if (inventoryUI.activeSelf == true)
        {
            InventorySlotInfo[] slots = playerINV.GetArrayOfItemsInInventory();
            ClearObjectChildren(inventorySlotsPanel);
            foreach (InventorySlotInfo s in slots)
            {
                GameObject t = Instantiate(slotTemplate, inventorySlotsPanel.transform);
                t.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = s.icon;
                t.transform.GetChild(1).gameObject.GetComponent<Text>().text = s.name;
                t.transform.GetChild(2).gameObject.GetComponent<Text>().text = "" + s.am;
            }
        }
    }

    public void CloseInv()
    {
        inventoryUI.SetActive(false);
        inventoryOpen = false;
    }

    public void OpenInv()
    {
        if (inventoryOpen == false)
        {
            inventoryOpen = true;
            inventoryUI.SetActive(true);
            UpdateInventoryUi();
        }
        else
        {
            CloseInv();
        }
        
    }

    #endregion


    public void Notify_success()
    {
        UpdateBackpackUi();
    }

    public void Notify_fail()
    {
        curenltySelectedToolbeltSlot = -1;
        UpdateBackpackUi();
    }

    private void ClearObjectChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
    }
    
    
}