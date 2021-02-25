using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public GameObject terrainObj;
    public GameObject playerObj;

    private TerrainGenerator terrain;
    private Grid grid;

    void Start()
    {
        terrain = terrainObj.GetComponent<TerrainGenerator>();
        grid = new Grid();
        grid.CreateGrid(terrain.xSize, terrain.ySize);
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

        terrain.RemoveTreeFromGrid((int)pos.x, (int)pos.z);
        return null;
    }
    
    


    #region PRIVATE METHODS BELLOW

    private Vector3 adjustCords(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + 0.01f);
        int y = Mathf.RoundToInt(pos.y + 0.01f);
        int z = Mathf.RoundToInt(pos.z + 0.01f);

        return new Vector3(x, y, z);
    }

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
                    Vector3 spawn = new Vector3(i, 1, j);
                    playerObj.transform.position = spawn;
                    return;
                }
            }
        }
    }
    #endregion


}
