using System.Collections.Generic;
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

    private float plants_MinBoundary = 0.01f;           // Regulates how far from water the plants begin to spawn
    private float plants_MaxBoundary = 0.01f;           // Regulates how far from the edges of forests and heights will plants begin to spawn
    private float plants_Density = 0.2f;    // How dense


    [Range(0, 100)]
    public int forest_PlantsToTrees_ratio = 10;     // Regulates how often plants should spawn in the foest
    // =========================


    // Water features settings
    private float WaterCoverage = 0.15f;            // Regulates perecentage of water covering the grass area
    // =========================


    // For Rendering and Testing (WILL BE REMOVED OR DISABLED)
    //[Range(0, 30)]
    private float UpdateTime = 0;
    // =========================


    // Terrain Layers (object locations) GameObject references
    public GameObject layer_Ground;                // Grass (ground) terrain parts stored here
    public GameObject layer_Rocks;                 // Walkable rocks (around mountains) terrain parts stored here
    public GameObject layer_Mountains;             // Mountains (unwalkable) terrain parts stored here
    public GameObject layer_Plants;                // Forests and *vegetation?* objects stored here
    public GameObject layer_Waters;                // Lakes, rivers terrain parts stored here

    public GameObject layer_parent_Terrain;        // Main parent where all layers stored

    // =========================


    // Terrain parts, objects GameObject references
    public GameObject block_Grass;
    public GameObject block_Foresty;
    public GameObject block_Harsh;
    public GameObject block_Sandy;

    public GameObject block_Rock;
    public GameObject block_Mountain;
    public GameObject block_Water;


    [Header("Trees Spawn Pool")] public GameObject[] spawner_Tree_Pool;
    [Header("Forest Small Plants Spawn Pool")] public GameObject[] spawner_ForestPlants_Pool;
    [Header("Plants Spawn Pool")] public GameObject[] spawner_Plants_Pool;
    // =========================


    // Will be Removed, track if UpdateTime ran out
    private float Elapsed = 0;
    // =========================


    // Temporary variables, will be changed
    private GameObject[,] grid_Terrain_Objects;       // Stores grass,rock,mountain terrain blocks
    private GameObject[,] grid_Vegetation_Objects;    // Stores trees,mushrooms,plants blocks

    private GameObject[,] grid_Interactable_Objects;  // Stores balloon pads and other stuf like that

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
        Generate_Plants(island);
    }

    #region world pre-loading
    void ClearMap()
    {
        ClearObjectChildren(layer_Ground);
        ClearObjectChildren(layer_Plants);
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
        grid_Interactable_Objects = new GameObject[sX, sY];
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
                grid_Interactable_Objects[i, j] = null;

                if (map[i, j] == 1)
                {

                    float p_val = GetPerlinVal(i, j, terrain_PerlinScale, terrain_XOffset, terrain_YOffset);

                    if (p_val <= WaterCoverage)
                    {
                        Spawner_Water(i, j);
                    }
                    else if (p_val >= MountainsCoverage * RockAmount && p_val < MountainsCoverage)
                    {
                        Spawner_FlatRock(i, j);
                    }
                    else if (p_val >= MountainsCoverage)
                    {
                        Spawner_Mountain(i, j, p_val);
                    }
                    else
                    {
                        Spawner_Grass(i, j);
                    }

                }
                else
                {
                    grid_Terrain_Objects[i, j] = null;
                }
            }
        }
    }

    void Generate_Forests(int[,] map)
    {
        bool spawn_Trees = (spawner_Tree_Pool.Length > 0);
        bool spawn_Plants = (spawner_ForestPlants_Pool.Length > 0);

        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                string current_block_tag = grid_Terrain_Objects[i, j].tag;
                float p_val = GetPerlinVal(i, j, forest_PerlinScale, forest_XOffset, forest_YOffset);

                if (map[i, j] == 1 && !current_block_tag.Equals("Mountain") && !current_block_tag.Equals("Water") && p_val <= ForestCoverage && (spawn_Trees || spawn_Plants))
                {

                    if (spawn_Trees == true && spawn_Plants == true)
                    {
                        int chance = Random.Range(0, 101);

                        if (chance <= forest_PlantsToTrees_ratio)
                        {
                            Spawner_ForestPlant(i, j);
                        }
                        else
                        {
                            Spawner_Tree(i, j);
                        }

                    }
                    else if (spawn_Trees == true)
                    {
                        Spawner_Tree(i, j);
                    }
                    else if (spawn_Plants == true)
                    {
                        Spawner_ForestPlant(i, j);
                    }
                }
                else
                {
                    grid_Vegetation_Objects[i, j] = null;
                }
            }
        }
    }

    void Generate_Plants(int[,] map)
    {
        bool spawn_Plants = (spawner_Plants_Pool.Length > 0);
        if (spawn_Plants)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 1)
                    {
                        string current_block_tag = grid_Terrain_Objects[i, j].tag;
                        float p_val_ter = GetPerlinVal(i, j, terrain_PerlinScale, terrain_XOffset, terrain_YOffset);
                        float p_val_veg = GetPerlinVal(i, j, forest_PerlinScale, forest_XOffset, forest_YOffset);

                        if (/*!current_block_tag.Equals("Mountain") &&
                            !current_block_tag.Equals("Water") &&
                            !current_block_tag.Equals("Rock") &&*/
                            p_val_ter > (WaterCoverage + plants_MinBoundary) && p_val_ter < ((MountainsCoverage * RockAmount) - plants_MaxBoundary) &&
                            p_val_veg > (ForestCoverage + plants_MaxBoundary))
                        {
                            float distMaxR = (MountainsCoverage * RockAmount) - ((MountainsCoverage * RockAmount) - plants_MaxBoundary);
                            float distMaxF = (ForestCoverage + plants_MaxBoundary) - (ForestCoverage);

                            float distMax = distMaxR;
                            if (distMaxR > distMaxF) distMax = distMaxF;

                            float distMin = (WaterCoverage + plants_MinBoundary) - WaterCoverage;

                            float prop = (distMax / distMin);// * plants_Density;
                            if (distMax > distMin) prop = distMin / distMax;

                            prop = prop * plants_Density;

                            int chance = Random.Range(0, 101);

                            if (chance <= prop * 100)
                            {
                                Spawner_Plant(i, j);
                            }
                        }

                    }
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
    // --------------------------------------------
    // Terrain
    private void Spawner_Grass(int x, int z)
    {
        GameObject obj = SpawnGrass(x, z);
        grid_Terrain_Objects[x, z] = obj;
    }

    private void Spawner_FlatRock(int x, int z)
    {
        GameObject obj = SpawnFlatRock(x, z);
        grid_Terrain_Objects[x, z] = obj;
    }

    private void Spawner_Mountain(int x, int z, float h)
    {
        GameObject obj = SpawnMountain(x, z, h);
        grid_Terrain_Objects[x, z] = obj;
    }

    private void Spawner_Water(int x, int z)
    {
        GameObject obj = SpawnWater(x, z);
        grid_Terrain_Objects[x, z] = obj;
    }

    // --------------------------------------------
    // Trees and plants
    private void Spawner_Tree(int x, int z)
    {
        if (grid_Vegetation_Objects[x, z] == null && IsPlantable(x, z))
        {
            string h = grid_Terrain_Objects[x, z].tag;
            GameObject obj = SpawnTree(x, z, h);
            grid_Vegetation_Objects[x, z] = obj;
        }
    }

    private void Spawner_ForestPlant(int x, int z)
    {
        string h = grid_Terrain_Objects[x, z].tag;
        GameObject obj = SpawnForestPlant(x, z, h);
        grid_Vegetation_Objects[x, z] = obj;
    }

    private void Spawner_Plant(int x, int z)
    {
        GameObject obj = SpawnPlant(x, z);
        grid_Vegetation_Objects[x, z] = obj;
    }

    // --------------------------------------------
    // Private 
    private GameObject SpawnGrass(int i, int j)
    {
        GameObject grass = null;

        float p_val_ter = GetPerlinVal(i, j, terrain_PerlinScale, terrain_XOffset, terrain_YOffset);
        float p_val_veg = GetPerlinVal(i, j, forest_PerlinScale, forest_XOffset, forest_YOffset);

        if (p_val_ter >= WaterCoverage && p_val_ter <= WaterCoverage + 0.1f && p_val_veg > ForestCoverage - 0.06f)
            grass = Instantiate(block_Sandy);
        else if (p_val_ter <= (MountainsCoverage * RockAmount) && p_val_ter >= (MountainsCoverage * RockAmount) - 0.08f)
            grass = Instantiate(block_Harsh);
        else if (p_val_veg <= ForestCoverage + 0.06f)
            grass = Instantiate(block_Foresty);
        else
            grass = Instantiate(block_Grass);

        grass.name = "Grass_" + i + "_" + j;
        grass.transform.parent = layer_Ground.transform;

        grass.transform.position = new Vector3(i, 0.5f, j);

        return grass;
    }

    private GameObject SpawnFlatRock(int i, int j)
    {
        GameObject rock = Instantiate(block_Rock);
        rock.name = "Rock_" + i + "_" + j;
        rock.transform.parent = layer_Rocks.transform;

        rock.transform.position = new Vector3(i, 0.75f, j);

        return rock;
    }

    private GameObject SpawnMountain(int i, int j, float z)
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


    private GameObject SpawnWater(int i, int j)
    {
        GameObject wat = Instantiate(block_Water);
        wat.name = "Wtr_" + i + "_" + j;
        wat.transform.parent = layer_Waters.transform;

        wat.transform.position = new Vector3(i, 0.5f, j);

        return wat;
    }

    private GameObject SpawnTree(int i, int j, string h)
    {
        int whichOne = Random.Range(0, spawner_Tree_Pool.Length);
        GameObject tree = Instantiate(spawner_Tree_Pool[whichOne]);
        tree.name = "Tree_" + i + "_" + j;
        tree.transform.parent = layer_Plants.transform;

        float height = 1f;
        if (h.Equals("Rock")) height = 1.5f;
        int rn = Random.Range(-6, 6);
        int rot = 90 * rn;

        tree.transform.position = new Vector3(i, height, j);
        tree.transform.rotation = new Quaternion(0, rot * (Mathf.PI / 180), 0, 1);

        return tree;
    }

    private GameObject SpawnForestPlant(int i, int j, string h)
    {
        int whichOne = Random.Range(0, spawner_ForestPlants_Pool.Length);
        GameObject plant = Instantiate(spawner_ForestPlants_Pool[whichOne]);

        plant.name = "FPlant_" + i + "_" + j;
        plant.transform.parent = layer_Plants.transform;

        float height = 1f;
        if (h.Equals("Rock")) height = 1.5f;
        int rn = Random.Range(-6, 6);
        int rot = 90 * rn;

        plant.transform.position = new Vector3(i, height, j);
        plant.transform.rotation = new Quaternion(0, rot * (Mathf.PI / 180), 0, 1);

        return plant;
    }

    private GameObject SpawnPlant(int i, int j)
    {
        int whichOne = Random.Range(0, spawner_Plants_Pool.Length);
        GameObject plant = Instantiate(spawner_Plants_Pool[whichOne]);

        plant.name = "Plant_" + i + "_" + j;
        plant.transform.parent = layer_Plants.transform;

        float height = 1f;
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
        GameObject located = grid_Terrain_Objects[i, j];          // Located Vegetation object on the surface of the terrain

        if (located != null)
        {
            return located;
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
        GameObject located = grid_Vegetation_Objects[i, j];                // Located Terrain object in the world

        if (located != null)
        {
            return located;
        }

        return null;
    }

    /// <summary>
    /// Checks if anything can be planted on the given X and Z
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns> true if anything can be planted there </returns>
    public bool IsPlantable(int i, int j)
    {
        GameObject located = grid_Vegetation_Objects[i, j];                // Located Terrain object in the world

        if (IsWalkable(i, j) && located == null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Cheks if there is a tree
    /// </summary>
    public bool IsTree(int i, int j)
    {
        GameObject located = grid_Vegetation_Objects[i, j];                // Located Terrain object in the world

        if (located != null && located.CompareTag("Tree"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Cheks if there is a shroom
    /// </summary>
    public bool IsShroom(int i, int j)
    {
        GameObject located = grid_Vegetation_Objects[i, j];                // Located Terrain object in the world

        if (located != null && located.CompareTag("Mushroom"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the area on given X and Z is walkable
    /// (if thre is a tree or water in that X and Z than it's unwalkable)
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    /// <returns> true if walkable, false if not</returns>
    public bool IsWalkable(int i, int j)
    {

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
    /// Checks if given X and Z pairs share same border (are right or left or up or down form each other)
    /// </summary>
    /// <param name="x1"> pair 1 x </param>
    /// <param name="z1"> pair 1 z </param>
    /// <param name="x2"> pair 2 x</param>
    /// <param name="z2"> pair 2 z </param>
    /// <returns> true if they share same border </returns>
    public bool IsAdjacent(int x1, int z1, int x2, int z2)
    {
        if ((x1 + 1 == x2 || x1 - 1 == x2) && z1 == z2) return true;
        if ((z1 + 1 == z2 || z1 - 1 == z2) && x1 == x2) return true;
        return false;
    }

    /// <summary>
    /// Checks if given X and Z pairs share at least one point (they are nearby each other)
    /// </summary>
    /// <param name="x1"> pair 1 x </param>
    /// <param name="z1"> pair 1 z </param>
    /// <param name="x2"> pair 2 x</param>
    /// <param name="z2"> pair 2 z </param>
    /// <returns> true if they share at least one point </returns>
    public bool IsAdjacent_diaganal(int x1, int z1, int x2, int z2)
    {
        if (IsAdjacent(x1, z1, x2, z2)) return true;
        if ((x1 + 1 == x2 && z1 + 1 == z2)) return true;
        if ((x1 - 1 == x2 && z1 - 1 == z2)) return true;
        if ((x1 + 1 == x2 && z1 - 1 == z2)) return true;
        if ((x1 - 1 == x2 && z1 + 1 == z2)) return true;

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
    /// Removes Plant Object from the grid on given X and Z
    /// </summary>
    /// <param name="i"> x coord </param>
    /// <param name="j"> z coord </param>
    public void RemovePlantFromGrid(int i, int j)
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

        grid_Interactable_Objects[(int)objPos.x, (int)objPos.z] = obj;
    }
    // =========================


    public void Spawn_Tree_At(int x, int z)
    {
        if (IsPlantable(x, z) && grid_Interactable_Objects[x, z] == null)
        {
            string h = grid_Terrain_Objects[x, z].tag;
            GameObject obj = SpawnTree(x, z, h);
            grid_Vegetation_Objects[x, z] = obj;
        }
    }
    #endregion
}