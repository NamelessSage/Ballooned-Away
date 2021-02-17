using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    public GameObject terrainObj;
    public GameObject playerObj;

    private TerrainGenerator terrain;


    void Start()
    {
        terrain = terrainObj.GetComponent<TerrainGenerator>();
        
        spawnPlayer();
    }



    // METHODS BELLOW

    /// <summary>
    /// Find terrain object (grass block, rock block, mountain, water...) on given Vector3 coordinate
    /// </summary>
    /// <param name="pos"> coordinate where to look for a terrain object </param>
    /// <returns> retrun the Object if found, retrun NULL if there is no object on that terrain coordinate </returns>
    public GameObject getTerrainObjectFromPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        return getTerrainFromPosition((int)pos.x, (int)pos.z);
    }


    /// <summary>
    /// Find object (flower, tree, rock...) on the surface of the terrain on given Vector3 coordinate
    /// </summary>
    /// <param name="pos">coordinate where to look for an object </param>
    /// <returns> retrun the Object if found, retrun NULL if there is no object on that terrain coordinate </returns>
    public GameObject getObjectFromPosition(Vector3 pos)
    {
        pos = adjustCords(pos);

        return getObjectFromTerrain((int)pos.x, (int)pos.z);
    }


    // PRIVATE METHODS BELLOW

    private Vector3 adjustCords(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x + 0.01f);
        int y = Mathf.RoundToInt(pos.y + 0.01f);
        int z = Mathf.RoundToInt(pos.z + 0.01f);

        return new Vector3(x, y, z);
    }

    private GameObject getObjectFromTerrain(int i, int j)
    {
        return terrain.getObjectFromTerrain(i, j);
    }

    private GameObject getTerrainFromPosition(int i, int j)
    {
        return terrain.getTerrainObject(i, j);
    }

    private void spawnPlayer()
    {
        var terrainObjects = terrain.getTerrainObjects();
        
        for (int i = terrainObjects.GetLength(0)/2; i < terrainObjects.GetLength(0); i++)
        {
            for (int j = 0; j < terrainObjects.GetLength(1); j++)
            {
                if (terrainObjects[i,j])
                {
                    if (terrainObjects[i,j].CompareTag("Grass"))
                    {
                        Vector3 spawn = new Vector3(i, 1, j);
                        playerObj.transform.position = spawn;
                        return;
                    }

                }
            }
        }
    }
}
