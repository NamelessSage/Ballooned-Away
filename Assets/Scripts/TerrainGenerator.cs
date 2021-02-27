﻿using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Created and documented by: Denis Tarasonis
    // TERRAIN GENERATION, TERRAIN ZONE CREATION
    // ===========================================

    #region Testing values used for map generation, CURRENTLY INACTIVE
    //// Primary terrain settings
    //[Range(0.0f, 1.0f)]
    //public float MountainsCoverage = 0.65f;         // Regulates amount of terrain heights
    //[Range(0.0f, 1.0f)]
    //public float RockAmount = 0.842f;               // Regulates how much of walkable rock is generated around heights
    //[Range(0.0f, 1.0f)]
    //public float terrain_PerlinScale = 0.148f;      // Regulates perlin noise scale (low - high -->> detailed but less features - abstract but more features)
    //[Range(-9000, 9000)]
    //public float terrain_XOffset = 0;               // Perlin noise X offset (Should be Random by Default)
    //[Range(-9000, 9000)]
    //public float terrain_YOffset = 0;               // Perlin noise Y offset (Should be Random by Default)
    //// =========================


    //// Forest and vegetation settings
    //[Range(0.0f, 1.0f)]
    //public float ForestCoverage = 0.472f;           // Regulates amount of forest generated over the terrain
    //[Range(0.0f, .99f)]
    //public float forest_PerlinScale = 0.148f;       // Regulates perlin noise scale (low - high -->> blobby and big forests - smaller and many forests)
    //[Range(-9000, 9000)]
    //public float forest_XOffset = 0;                // Perlin noise X offset (Should be Random by Default)
    //[Range(-9000, 9000)]
    //public float forest_YOffset = 0;                // Perlin noise Y offset (Should be Random by Default)
    //// =========================


    //// Water features settings
    //[Range(0.0f, 1.0f)]
    //public float WaterCoverage = 0.65f;
    //// =========================

    //=============================================
    // TESITNG END /\/\/\/\/\/\/\/\/\
    //=============================================
    #endregion


    // Terrain size
    public int xSize;
    public int ySize;
    // =========================

    // Primary terrain settings
    private float MountainsCoverage = 0.7f;          // Regulates amount of terrain heights
    private float RockAmount = 0.83f;                // Regulates how much of walkable rock is generated around heights
    private float terrain_PerlinScale = 0.148f;      // Regulates perlin noise scale (low - high -->> detailed but less features - abstract but more features)
    private float terrain_XOffset = 0;               // Perlin noise X offset (Should be Random by Default)
    private float terrain_YOffset = 0;               // Perlin noise Y offset (Should be Random by Default)
    // =========================


    // Forest and vegetation settings
    private float ForestCoverage = 0.466f;           // Regulates amount of forest generated over the terrain
    private float forest_PerlinScale = 0.148f;       // Regulates perlin noise scale (low - high -->> blobby and big forests - smaller and many forests)
    private float forest_XOffset = 0;                // Perlin noise X offset (Should be Random by Default)
    private float forest_YOffset = 0;                // Perlin noise Y offset (Should be Random by Default)

    [Range(0, 100)]
    public int forest_PlantsToTrees_ratio = 10;     // Regulates how often plants should spawn in the foest
    // =========================


    // Water features settings
    private float WaterCoverage = 0.15f;
    // =========================


    // For Rendering and Testing (WILL BE REMOVED OR DISABLED)
    //[Range(0, 30)]
    private float UpdateTime = 0;
    // =========================


    // Terrain Layers (object locations) GameObject references
    public GameObject layer_Ground;                // Grass (ground) terrain parts stored here
    public GameObject layer_Rocks;                 // Walkable rocks (around mountains) terrain parts stored here
    public GameObject layer_Mountains;             // Mountains (unwalkable) terrain parts stored here
    public GameObject layer_Forests;               // Forests and *vegetation?* objects stored here
    public GameObject layer_Waters;                // Lakes, rivers terrain parts stored here

    public GameObject layer_parent_Terrain;        // Main parent where all layers stored

    // =========================


    // Terrain parts, objects GameObject references
    public GameObject block_Grass;
    public GameObject block_Rock;
    public GameObject block_Mountain;
    public GameObject block_Water;


    [Header("Trees Spawn Pool")] public GameObject[] spawner_Tree_Pool;
    [Header("Forest Small Plants Spawn Pool")] public GameObject[] spawner_Plants_Pool;
    // =========================


    // Will be Removed, track if UpdateTime ran out
    private float Elapsed = 0;
    // =========================


    // Dictionaries to keep terrain parts and objects name tags
    private Dictionary<string, int> types = new Dictionary<string, int>()
    {
        {"dead", -1},
        {"grass", 0},
        {"rock", 1},
        {"mountain", 2 },
        {"water", 3 } // water, for now
    };

    private Dictionary<string, int> vegTypes = new Dictionary<string, int>()
    {
        {"dead", -1},
        {"plain", 0},
        {"grass", 1},
        {"tree", 2}
    };
    // =========================


    // Temporary variables, will be changed
    private GameObject[,] grid_Terrain_Objects;          // Stores grass,rock,mountain terrain blocks
    private GameObject[,] grid_Vegetation_Objects;    // Stores trees,mushrooms,plants blocks

    //private int[,] grid_Terrain_Array;          // Stores terrain grid tags 
    //private int[,] grid_Vegetation_Array;       // Stores vegetation grid tags


    // ===========================



    // ===========================

    // Main Logic
    void Start()
    {
        Elapsed = UpdateTime;
        ResetArrays(0, 0);
        CalculateOffsets();
        InitiateIsland(xSize, ySize);
    }

    void Update()
    {
        //InitializeWorld();
    }

    void InitializeWorld()
    {
        Elapsed = Elapsed - Time.deltaTime;
        if (Elapsed <= 0)
        {
            Elapsed = UpdateTime;

            InitiateIsland(xSize, ySize);
        }

    }
    // =========================

    void InitiateIsland(int x_s, int y_s)
    {
        ClearMap();

        int[,] island = GenerateSquareIsland(x_s, y_s);
        xSize = island.GetLength(0);
        ySize = island.GetLength(1);
        ResetArrays(xSize, ySize);
        Generate_Surface(island);
        Generate_Forests(island);
    }

    #region world pre-loading
    void ClearMap()
    {
        ClearObjectChildren(layer_Ground);
        ClearObjectChildren(layer_Forests);
        ClearObjectChildren(layer_Rocks);
        ClearObjectChildren(layer_Mountains);
        ClearObjectChildren(layer_Waters);
    }

    void CalculateOffsets()
    {
        forest_XOffset = Random.Range(-9000, 9000);
        forest_YOffset = Random.Range(-9000, 9000);
        terrain_YOffset = Random.Range(-9000, 9000);
        terrain_XOffset = Random.Range(-9000, 9000);
    }

    void ResetArrays(int sX, int sY)
    {
        grid_Terrain_Objects = new GameObject[sX, sY];
        grid_Vegetation_Objects = new GameObject[sX, sY];

        //grid_Terrain_Array = new int[sX, sY];
        //grid_Vegetation_Array = new int[sX, sY];
    }

    int[,] GenerateSquareIsland(int size_x, int size_y)
    {
        int[,] layout = new int[size_x, size_y];
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                layout[i, j] = 1;
            }
        }

        return layout;
    }
    #endregion


    #region World generation methdos
    void Generate_Surface(int[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == 1)
                {

                    float z = GetPerlinVal(i, j, terrain_PerlinScale, terrain_XOffset, terrain_YOffset);

                    GameObject newBlock;

                    if (z <= WaterCoverage)
                    {
                        newBlock = SpawnWater(i, j);
                        //grid_Terrain_Array[i, j] = types["water"];
                    }
                    else if (z >= MountainsCoverage * RockAmount && z < MountainsCoverage)
                    {
                        newBlock = SpawnFlatRock(i, j);
                        //grid_Terrain_Array[i, j] = types["rock"];
                        //grid_Vegetation_Array[i, j] = vegTypes["plain"];
                    }
                    else if (z >= MountainsCoverage)
                    {
                        newBlock = SpawnMountain(i, j, z);
                        //grid_Terrain_Array[i, j] = types["mountain"];
                    }
                    else
                    {
                        newBlock = SpawnGrass(i, j);
                       // grid_Terrain_Array[i, j] = types["grass"];
                       // grid_Vegetation_Array[i, j] = vegTypes["plain"];
                    }

                    grid_Terrain_Objects[i, j] = newBlock;
                }
                else
                {
                    grid_Terrain_Objects[i, j] = null;
                   // grid_Terrain_Array[i, j] = types["dead"];
                }
            }
        }
    }

    void Generate_Forests(int[,] map)
    {
        bool spawn_Trees = (spawner_Tree_Pool.Length > 0);
        bool spawn_Plants = (spawner_Plants_Pool.Length > 0);

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                // int h = grid_Terrain_Array[i, j];
                string h = grid_Terrain_Objects[i, j].tag;
                float z = GetPerlinVal(i, j, forest_PerlinScale, forest_XOffset, forest_YOffset);

                if (map[i, j] == 1 && !h.Equals("Mountain") && !h.Equals("Water") && z <= ForestCoverage && (spawn_Trees || spawn_Plants))
                {

                    if (spawn_Trees == true && spawn_Plants == true)
                    {
                        GameObject newPlant;
                        int chance = Random.Range(0, 101);

                        if (chance <= forest_PlantsToTrees_ratio)
                        {
                            newPlant = SpawnForestPlant(i, j, h);
                            //grid_Vegetation_Array[i, j] = vegTypes["grass"];
                        }
                        else
                        {
                            newPlant = SpawnTree(i, j, h);
                            //grid_Vegetation_Array[i, j] = vegTypes["tree"];
                        }

                        grid_Vegetation_Objects[i, j] = newPlant;
                    }
                    else if (spawn_Trees == true)
                    {
                        GameObject newPlant;
                        newPlant = SpawnTree(i, j, h);
                        //grid_Vegetation_Array[i, j] = vegTypes["tree"];
                    }
                    else if (spawn_Plants == true)
                    {
                        GameObject newPlant;
                        newPlant = SpawnTree(i, j, h);
                        //grid_Vegetation_Array[i, j] = vegTypes["tree"];
                    }
                }
                else
                {
                    grid_Vegetation_Objects[i, j] = null;
                    //grid_Vegetation_Array[i, j] = vegTypes["dead"];
                }
            }
        }
    }
    #endregion


    #region Perlin Noise array creation and Perlin noise value calculator
    float[,] GeneratePerlinNoiseArray(int size_x, int size_y, float multiplier, int x_offset, int y_offset)
    {
        float[,] noiseArray = new float[size_x, size_y];

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                float perlinVal = GetPerlinVal(i, j, multiplier, x_offset, y_offset);
                noiseArray[i, j] = perlinVal;
            }
        }

        return noiseArray;
    }

    float GetPerlinVal(int i, int j, float mult, float xO, float yO)
    {
        float xVal = (float)i * mult + xO;
        float yVal = (float)j * mult + yO;

        float val = Mathf.PerlinNoise(xVal, yVal);

        return val;
    }
    #endregion


    #region Terrain, plants, trees ... objects spanwer functions
    GameObject SpawnGrass(int i, int j)
    {
        GameObject grass = Instantiate(block_Grass);
        grass.name = "Grass_" + i + "_" + j;
        grass.transform.parent = layer_Ground.transform;

        grass.transform.position = new Vector3(i, 0.5f, j);

        return grass;
    }

    GameObject SpawnFlatRock(int i, int j)
    {
        GameObject rock = Instantiate(block_Rock);
        rock.name = "Rock_" + i + "_" + j;
        rock.transform.parent = layer_Rocks.transform;

        rock.transform.position = new Vector3(i, 0.75f, j);

        return rock;
    }

    GameObject SpawnMountain(int i, int j, float z)
    {
        GameObject mnt = Instantiate(block_Mountain);
        mnt.name = "Mnt_" + i + "_" + j;
        mnt.transform.parent = layer_Mountains.transform;

        float difference = z - MountainsCoverage + 1;

        float newMultiplier = Mathf.Pow(z * (difference + 1), z * (difference + 1));
        float newHeight = 2 * newMultiplier;
        newHeight = newHeight * 0.6f; // Decreasing height by x%

        float newY = newHeight / 2;

        mnt.transform.localScale = new Vector3(1, newHeight, 1);
        mnt.transform.position = new Vector3(i, newY, j);

        return mnt;
    }


    GameObject SpawnWater(int i, int j)
    {
        GameObject wat = Instantiate(block_Water);
        wat.name = "Wtr_" + i + "_" + j;
        wat.transform.parent = layer_Waters.transform;

        wat.transform.position = new Vector3(i, 0.5f, j);

        return wat;
    }

    GameObject SpawnTree(int i, int j, string h)
    {
        int whichOne = Random.Range(0, spawner_Tree_Pool.Length);
        GameObject tree = Instantiate(spawner_Tree_Pool[whichOne]);
        tree.name = "Tree_" + i + "_" + j;
        tree.transform.parent = layer_Forests.transform;

        float height = 1f;
        if (h.Equals("Rock")) height = 1.5f;
        int rn = Random.Range(-6, 6);
        int rot = 90 * rn;

        tree.transform.position = new Vector3(i, height, j);
        tree.transform.rotation = new Quaternion(0, rot * (Mathf.PI / 180), 0, 1);

        return tree;
    }

    GameObject SpawnForestPlant(int i, int j, string h)
    {
        int whichOne = Random.Range(0, spawner_Plants_Pool.Length);
        GameObject plant = Instantiate(spawner_Plants_Pool[whichOne]);

        plant.name = "FPlant_" + i + "_" + j;
        plant.transform.parent = layer_Forests.transform;

        float height = 1f;
        if (h.Equals("Rock")) height = 1.5f;
        int rn = Random.Range(-6, 6);
        int rot = 90 * rn;

        plant.transform.position = new Vector3(i, height, j);
        plant.transform.rotation = new Quaternion(0, rot * (Mathf.PI / 180), 0, 1);

        return plant;
    }
    #endregion


    #region  Local tools, useful functions
    private void ClearObjectChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
    }

    private int findMax(int[] array)
    {
        int a = 1;
        for (int i = 1; i < array.Length; i++)
        {
            if (array[a] < array[i])
            {
                a = i;
            }
        }
        return a;
    }
    // =========================
    #endregion


    #region INTERFACE METHODS for other scripts and connectivity with other modules
    public void PrintMe(int a)
    {
        Debug.Log("Ive been printed from TerrainGenerator: " + a);
    }


    /// <summary>
    /// Finds and return a terrian block (Grass block or rock block)
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns> GameObject or null if nothingn found on guven X anz Z </returns>
    public GameObject Get_Terrian_Object_From_Grid(int i, int j)
    {
        GameObject locatedVeg = grid_Terrain_Objects[i, j];          // Located Vegetation object on the surface of the terrain

        if (locatedVeg != null)
        {
            return locatedVeg;
        }

        return null;
    }

    /// <summary>
    /// Finds and return a vegetation object (Tree or mushrooms or grass...)
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns> GameObject or null if nothingn found on guven X anz Z </returns>
    public GameObject Get_Vegetation_Object_From_Grid(int i, int j)
    {
        GameObject locatedTer = grid_Vegetation_Objects[i, j];                // Located Terrain object in the world

        if (locatedTer != null)
        {
            return locatedTer;
        }

        return null;
    }

    /// <summary>
    /// Checks if the area on given X and Z is walkable
    /// (if thre is a tree or water in that X and Z than it's unwalkable)
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns> true if walkable, false if not</returns>
    public bool GetWalkable(int i, int j)
    {
        // Debug.Log(i + " " + j);
        // Debug.Log(grid_Terrain_Array[i,j]);

        if (i < 0 || i >= xSize || j < 0 || j >= ySize || grid_Terrain_Objects[i, j] == null)
            return false;

        if ((grid_Terrain_Objects[i, j].tag.Equals("Grass") || grid_Terrain_Objects[i, j].tag.Equals("Rock")) &&
            (grid_Vegetation_Objects[i, j] == null || !grid_Vegetation_Objects[i, j].tag.Equals("Tree")))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if on the given X and Z there is grass block and no unwalkable objects on top of it
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns></returns>
    public bool ChechkIfFlat(int i, int j)
    {
        // Debug.Log(i + " " + j);
        // Debug.Log(grid_Terrain_Array[i,j]);

        if (i < 0 || i >= xSize || j < 0 || j >= ySize || grid_Terrain_Objects[i, j] == null)
            return false;

        if (grid_Terrain_Objects[i, j].tag.Equals("Grass") &&
            (grid_Vegetation_Objects[i, j] == null))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes Tree Object from the grid on given X and Z
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    public void RemoveTreeFromGrid(int i, int j)
    {
        //grid_Vegetation_Array[i, j] = vegTypes["dead"];
        grid_Vegetation_Objects[i, j] = null;
    }

    /// <summary>
    /// Return whole Terrain Blocks grid
    /// </summary>
    /// <returns></returns>
    public GameObject[,] GetTerrainObjects()
    {
        return grid_Terrain_Objects;
    }

    /// <summary>
    /// Return whole Vegetation Objects grid
    /// </summary>
    /// <returns></returns>
    public GameObject[,] GetVegetationObjects()
    {
        return grid_Vegetation_Objects;
    }

    /// <summary>
    /// Positions a given object in given X and Z in the world
    /// </summary>
    /// <param name="obj"> the object </param>
    /// <param name="objPos"> desired grid position (X and Z must be integers) </param>
    public void PositionateObjectInWorld(GameObject obj, Vector3 objPos)
    {
        float newY = grid_Terrain_Objects[(int)objPos.x, (int)objPos.z].transform.position.y + 0.5f;
        obj.transform.position = new Vector3(objPos.x, newY, objPos.z);
    }
    // =========================
    #endregion
}