using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    // External modules
    public GameObject terrainObj;
    public GameObject playerObj;
    public GameObject uiObj;

    private TerrainGenerator terrain;
    private PlayerGuiController GUI;
    private AcquirableAssetsData ResourseAndItemManager;
    private Inventory PlrInventory;
    private BalloonPad Ballon_pad_script;

    //-------------------------------------


    // Mechanics objects
    public GameObject balloon_Pad_Prefab;
    public GameObject balloon_Model;

    private GameObject Balloon_Pad;
    //


    void Start()
    {
        terrain = terrainObj.GetComponent<TerrainGenerator>();
        GUI = uiObj.GetComponent<PlayerGuiController>();
        ResourseAndItemManager = GetComponent<AcquirableAssetsData>();
        PlrInventory = GetComponent<Inventory>();

        spawnPlayer();
        spawnBalloonPad();
    }


    //------------------------------------------

    public void UpdateWoodAmount()
    {
        int i = Random.Range(1, 10);
        PlrInventory.Resources_AddToResources("Wood", i);
    }


    public Vector3 adjustCords(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + 0.01f);
        int y = Mathf.RoundToInt(pos.y + 0.01f);
        int z = Mathf.RoundToInt(pos.z + 0.01f);

        return new Vector3(x, y, z);
    }

    public void OpenShopUI()
    {
        GUI.OpeShop();
    }


    // METHODS BELLOW



    #region Terrain Interface Methods
    /// <summary>
    /// Find terrain object (grass block, rock block, mountain, water...) on given Vector3 coordinate
    /// </summary>
    /// <param name="pos"> coordinate where to look for a terrain object </param>
    /// <returns> retrun the Object if found, retrun NULL if there is no object on that terrain coordinate </returns>
    public GameObject GetTerrainObjectFromPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        return terrain.Get_Terrian_Object_From_Grid((int)pos.x, (int)pos.z);
    }


    /// <summary>
    /// Find object (flower, tree, rock...) on the surface of the terrain on given Vector3 coordinate
    /// </summary>
    /// <param name="pos">coordinate where to look for an object </param>
    /// <returns> retrun the Object if found, retrun NULL if there is no object on that terrain coordinate </returns>
    public GameObject GetVegetationObjectFromPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        return terrain.Get_Vegetation_Object_From_Grid((int)pos.x, (int)pos.z);
    }


    /// <summary>
    /// Return Terrain Generator object
    /// </summary>
    /// <returns> TerrainGenerator object </returns>
    public TerrainGenerator GetTerrain()
    {
        return terrain;
    }

    /// <summary>
    /// Return Terrain Generator object
    /// </summary>
    /// <returns> TerrainGenerator object </returns>
    public PlayerGuiController GetUi()
    {
        return GUI;
    }


    /// <summary>
    /// Removes tree from the world grid
    /// </summary>
    /// <param name="pos"> position where tree is being removed Vector3</param>
    /// <returns> return null, will be changed </returns>
    public GameObject ChopDownTreeAtPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        terrain.RemoveTreeFromGrid((int)pos.x, (int)pos.z);
        return null;
    }
    #endregion

    #region PRIVATE METHODS BELLOW
    /// <summary>
    /// spawns a player where there on grass ant there is no vegetation
    /// </summary>
    private void spawnPlayer()
    {
        for (int i = (int)terrain.xSize/2; i < terrain.xSize; i++)
        {
            for (int j = 0; j < terrain.ySize; j++)
            {
                
                if (terrain.ChechkIfFlat(i, j))
                {
                    Vector3 spawn = new Vector3(i, 1.2f, j);
                    playerObj.transform.position = spawn;
                    return;
                }
            }
        }
    }

    private void spawnBalloonPad()
    {
        for (int i = ((int)terrain.xSize / 2); i < terrain.xSize; i++)
        {
            for (int j = 0; j < terrain.ySize; j++)
            {

                if (terrain.ChechkIfFlat(i, j))
                {

                    Vector3 pos = new Vector3(i, 0, j);
                    Balloon_Pad = Instantiate(balloon_Pad_Prefab);
                    Ballon_pad_script = Balloon_Pad.GetComponent<BalloonPad>();
                    Ballon_pad_script.SetPlayerUI(GUI);
                    Ballon_pad_script.SetPlayerInventory(PlrInventory);
                    Balloon_Pad.name = "Ballon Pad";
                    terrain.PositionateObjectInWorld(Balloon_Pad, pos);
                    return;
                }
            }
        }
    }
    #endregion
    

}
