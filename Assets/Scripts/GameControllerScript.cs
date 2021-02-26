using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public GameObject terrainObj;
    public GameObject playerObj;

    // player resources
    public GameObject playerUiObj;
    private UI PlayerUI;
    private int woodAmount = 0;
    private int rockAmount = 0;


    private TerrainGenerator terrain;

    void Start()
    {
        terrain = terrainObj.GetComponent<TerrainGenerator>();
        PlayerUI = playerUiObj.GetComponent<UI>();
        spawnPlayer();
    }



    // METHODS BELLOW

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
    /// Removes tree from the world grid
    /// </summary>
    /// <param name="pos"> position where tree is being removed Vector3</param>
    /// <returns> return null, will be changed </returns>
    public GameObject ChopDownTreeAtPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        woodAmount++;
        PlayerUI.printAmountOfWood(woodAmount);

        terrain.RemoveTreeFromGrid((int)pos.x, (int)pos.z);
        return null;
    }


    public Vector3 adjustCords(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + 0.01f);
        int y = Mathf.RoundToInt(pos.y + 0.01f);
        int z = Mathf.RoundToInt(pos.z + 0.01f);

        return new Vector3(x, y, z);
    }


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
                
                if (terrain.GetWalkable(i, j) && terrain.Get_Terrian_Object_From_Grid(i, j).transform.localScale.y <= 1.1f)
                {
                    Vector3 spawn = new Vector3(i, 1.2f, j);
                    playerObj.transform.position = spawn;
                    return;
                }
            }
        }
    }
    #endregion


}
