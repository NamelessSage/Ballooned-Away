using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Created and documented by: Denis Tarasonis
    // TERRAIN GENERATION, TERRAIN ZONE CREATION
    // ===========================================


    // Terrain size
    public int xSize;
    public int ySize;
    // =========================


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

    // Primary terrain settings
    private float MountainsCoverage = 0.7f;         // Regulates amount of terrain heights
    private float RockAmount = 0.83f;               // Regulates how much of walkable rock is generated around heights
    private float terrain_PerlinScale = 0.148f;      // Regulates perlin noise scale (low - high -->> detailed but less features - abstract but more features)
    private float terrain_XOffset = 0;               // Perlin noise X offset (Should be Random by Default)
    private float terrain_YOffset = 0;               // Perlin noise Y offset (Should be Random by Default)
    // =========================


    // Forest and vegetation settings
    private float ForestCoverage = 0.466f;           // Regulates amount of forest generated over the terrain
    private float forest_PerlinScale = 0.148f;       // Regulates perlin noise scale (low - high -->> blobby and big forests - smaller and many forests)
    private float forest_XOffset = 0;                // Perlin noise X offset (Should be Random by Default)
    private float forest_YOffset = 0;               // Perlin noise Y offset (Should be Random by Default)
    // =========================


    // Water features settings
    private float WaterCoverage = 0.15f;
    // =========================



    // Terrain Layers (object locations) GameObject references
    public GameObject layer_Ground;                // Grass (ground) terrain parts stored here
    public GameObject layer_Rocks;                 // Walkable rocks (around mountains) terrain parts stored here
    public GameObject layer_Mountains;             // Mountains (unwalkable) terrain parts stored here
    public GameObject layer_Forests;               // Forests and *vegetation?* objects stored here
    public GameObject layer_Waters;                // Lakes, rivers terrain parts stored here

    [SerializeField]
    private GameObject layer_parent_Terrain;        // Main parent where all layers stored
    // =========================


    // Terrain parts, objects GameObject references
    public GameObject block_Grass;
    public GameObject block_Rock;
    public GameObject block_Mountain;
    public GameObject block_Water;                  // Water block

    public GameObject object_Forest;               // Tree....

    
    // =========================


    // Code and other variables bellow

    // Will be Removed, track if UpdateTime ran out
    private float Elapsed = 0;
    // =========================


    // Refference, will be Removed
    /*
     * 0 - grass
     * 1 - forest
     * 2 - rock
     * 3 - mountain
     */
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
    private GameObject[,] TerrainObjs;
    private GameObject[,] VegetationObjects;

    private int[,] TerrainArray;
    private int[,] VegetationArray; // Vegetaion ground layer array
    // =========================

    
    // Main Logic
    void Start()
    {
        ResetArrays(0, 0);

        InitiateIsland(xSize, ySize);
    }

    //void Update()
    //{
    //    InitializeWorld();
    //}

    void InitializeWorld()
    {

        InitiateIsland(xSize, ySize);

    }
    // =========================

    void InitiateIsland(int x_s, int y_s)
    {
        ClearMap();

        int[,] bap  = GenerateIsland(x_s, y_s);
        xSize = bap.GetLength(0);
        ySize = bap.GetLength(1);
        int sX = bap.GetLength(0);
        int sY = bap.GetLength(1);
        ResetArrays(sX, sY);

        CalculateOffsets();

        Generate_Surface(bap);
        Generate_Forests(bap);
        

    }

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
        TerrainObjs = new GameObject[sX, sY];
        VegetationObjects = new GameObject[sX, sY];

        TerrainArray = new int[sX, sY];
        VegetationArray = new int[sX, sY];
    }

    int[,] GenerateIsland(int size_x, int size_y)
    {
        /*
         *            scale - 0.061
         *        threshold - 0.648
         * recommended size - 50x50
         * 
         * new remoneded:
         * size 150 x 150
         * mult - 0.008
         * thr - 0.953
         */

        float multiplier = 0.061f;
        float threshold = 0.648f;

        //float multiplier = 0.008f;
        //float threshold = 0.8f;



        int[,] layout = new int[size_x, size_y];

        float[,] perlinArray = GeneratePerlinNoiseArray(size_x, size_y, multiplier, (int)Random.Range(-9000, 9000), (int)Random.Range(-9000, 9000));

        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {

                Vector3 p1 = new Vector3(i, 0, j);
                Vector3 c = new Vector3(size_x / 2, 0, size_y / 2);

                int radius = size_x / 2;
                float dist = radius - (float)(radius * 0.2f);

                int layout_Val = 0;
                if (Vector3.Distance(p1, c) < dist && perlinArray[i, j] > threshold)
                {
                    layout_Val = 1;

                    //GameObject newBlock = Instantiate(mesh_MainBlock);
                    //newBlock.name = i + "_" + j;
                    //newBlock.transform.parent = layer_Ground.transform;
                    //newBlock.GetComponent<MeshRenderer>().material = material_Grass;
                    //newBlock.transform.position = new Vector3(i, 0, j);
                }

                layout[i, j] = layout_Val;
            }
        }

        layout = getIslands(layout);

        int[] colorData = maxColor(layout);
        //int count = getArea(layout, color[0]);
       

        if (colorData[1] < 200 || colorData[1] > 400)
        {
            return GenerateIsland(size_x, size_y);

        }
        else
        {
            // Shape Boundries
            int xf = -1; int xl = -1;  // higest X, lowest X;
            int yf = -1; int yl = -1;  // most-left Y, most-right Y

            for (int i = 0; i < size_x; i++)
            {
                for (int j = 0; j < size_y; j++)
                {
                    if (layout[i, j] == colorData[0])
                    {
                        layout[i, j] = 1;

                        // finidng boundries of the shape
                        // higest X, lowest X;
                        // most-left Y, most-right Y

                        if (xf == -1) { xf = i; }
                        xl = i;

                        if (yf == -1 || j < yf) { yf = j; }
                        if (yl == -1 || j > yl) { yl = j; }

                    }
                    else
                    {
                        layout[i, j] = 0;
                    }
                }
            }

            xl++; yl++;

            int isoX = xl - xf;
            int isoY = yl - yf;

            int[,] isolated_island = new int[isoX, isoY];

            for (int i = 0; i < isoX; i++)
            {
                for (int j = 0; j < isoY; j++)
                {
                    int newX = i + xf; int newY = j + yf;
                    isolated_island[i, j] = layout[newX, newY];
                }
            }

            int enlarge = 3;
            int finalX = isoX * enlarge;
            int finalY = isoY * enlarge;

            int[,] finalMap = new int[finalX, finalY];

            for (int i = 0; i < isoX; i++)
            {
                for (int j = 0; j < isoY; j++)
                {
                    for (int p = 0; p < enlarge; p++)
                    {
                        for (int b = 0; b < enlarge; b++)
                        {
                            finalMap[(i * enlarge) + p, (j * enlarge) + b] = isolated_island[i, j];

                        }
                    }
                }
            }

            return finalMap;

        }

    }

    int[] maxColor(int[,] layout)
    {
        int[] colors = new int[50];
        for (int s = 0; s < colors.Length; s++)
        {
            colors[s] = 0;
        }

        for (int i = 0; i < layout.GetLength(0); i++)
        {
            for (int j = 0; j < layout.GetLength(1); j++)
            {
                colors[layout[i, j]]++;
            }
        }

        int maxColor = findMax(colors);
        return new int[]{maxColor, colors[maxColor]};
    }

    int[,] getIslands(int[,] layout)
    {
        int colorVal = 2;
        for (int i = 0; i < layout.GetLength(0); i++)
        {
            int j = hitEdge(layout, i);
            if (j > -1)
            {
                layout = FloodFill(layout, i, j, 1, colorVal);
                colorVal++;
                i--;
            }
        }


        return layout;
    }

    int hitEdge(int[,] layout, int i)
    {

        for (int j = 0; j < layout.GetLength(1); j++)
        {
            if (layout[i, j] == 1)
                return j;
        }


        return -1;
    }


    int[,] FloodFill(int[,] layout, int pX, int pY, int targetColor, int replacementColor)
    {
        Stack<int> pixelsX = new Stack<int>();
        Stack<int> pixelsY = new Stack<int>();

        pixelsX.Push(pX);
        pixelsY.Push(pY);

        int Width = layout.GetLength(0);
        int Height = layout.GetLength(1);
        while (pixelsX.Count > 0)
        {
            int a = pixelsX.Pop();
            int b = pixelsY.Pop();
            if (a < Width && a > 0 &&
                    b < Height && b > 0)//make sure we stay within bounds
            {

                if (layout[a, b] == targetColor)
                {
                    layout[a, b] = replacementColor;
                    pixelsX.Push(a - 1);
                    pixelsY.Push(b);

                    pixelsX.Push(a + 1);
                    pixelsY.Push(b);

                    pixelsX.Push(a);
                    pixelsY.Push(b - 1);

                    pixelsX.Push(a);
                    pixelsY.Push(b + 1);
                }
            }
        }

        return layout;
    }

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
                        TerrainArray[i, j] = types["water"];
                        VegetationArray[i, j] = vegTypes["dead"];
                    }
                    else if (z >= MountainsCoverage * RockAmount && z < MountainsCoverage)
                    {
                        newBlock = SpawnSemiMountain(i, j);
                        TerrainArray[i, j] = types["rock"];
                        VegetationArray[i, j] = vegTypes["plain"];
                    }
                    else if (z >= MountainsCoverage)
                    {
                        newBlock = SpawnMountain(i, j, z);
                        TerrainArray[i, j] = types["mountain"];
                        VegetationArray[i, j] = vegTypes["dead"];
                    }
                    else
                    {
                        newBlock = SpawnGrass(i, j);
                        TerrainArray[i, j] = types["grass"];
                        VegetationArray[i, j] = vegTypes["plain"];
                    }

                    TerrainObjs[i, j] = newBlock;
                }
                else
                {
                    TerrainObjs[i, j] = null;
                    TerrainArray[i, j] = types["dead"];
                }
            }
        }
    }

    void Generate_Forests(int[,] map)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int h = TerrainArray[i, j];

                if (map[i, j] == 1 && h != types["mountain"] && h != types["water"])
                {
                    float z = GetPerlinVal(i, j, forest_PerlinScale, forest_XOffset, forest_YOffset);

                    if (z <= ForestCoverage)
                    {
                        VegetationArray[i, j] = vegTypes["tree"];
                        VegetationObjects[i, j] = SpawnTree(i, j, h);
                    }
                }
                else
                {
                    VegetationObjects[i, j] = null;
                    VegetationArray[i, j] = vegTypes["dead"];
                }
            }
        }
    }

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

    GameObject SpawnGrass(int i, int j)
    {
        GameObject grass = Instantiate(block_Grass);
        grass.name = "Grass_" + i + "_" + j;
        grass.tag = "Grass";
        grass.transform.parent = layer_Ground.transform;

        grass.transform.position = new Vector3(i, 0.5f, j);

        return grass;
    }

    GameObject SpawnSemiMountain(int i, int j)
    {
        GameObject rock = Instantiate(block_Rock);
        rock.name = "Rock_" + i + "_" + j;
        rock.tag = "Rock";
        rock.transform.parent = layer_Rocks.transform;

        rock.transform.position = new Vector3(i, 0.75f, j);

        return rock;
    }

    GameObject SpawnMountain(int i, int j, float z)
    {
        GameObject mnt = Instantiate(block_Mountain);
        mnt.name = "Mnt_" + i + "_" + j;
        mnt.tag = "Mountain";
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
        wat.tag = "Water";
        wat.transform.parent = layer_Waters.transform;

        wat.transform.position = new Vector3(i, 0.5f, j);

        return wat;
    }

    GameObject SpawnTree(int i, int j, int h)
    {
        GameObject tree = Instantiate(object_Forest);
        tree.name = "Tree_" + i + "_" + j;
        tree.tag = "Tree";
        tree.transform.parent = layer_Forests.transform;

        float height = 1.5f;
        if (h == types["rock"]) height = 2f;
        int rn = Random.Range(-6, 6);
        int rot = 90 * rn;

        tree.transform.position = new Vector3(i, height, j);
        tree.transform.rotation = new Quaternion(0, rot * (Mathf.PI / 180), 0, 1);

        return tree;
    }


    // Local tools, useful functions
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



    // INTERFACE METHODS
    public void PrintMe(int a)
    {
        Debug.Log("Ive been printed from TerrainGenerator: " + a);
    }


    public GameObject getObjectFromTerrain(int i, int j)
    {
        GameObject locatedVeg = VegetationObjects[i, j];          // Located Vegetation object on the surface of the terrain

        if (locatedVeg != null)
        {
            return locatedVeg;
        }

        return null;
    }

    public GameObject getTerrainObject(int i, int j)
    {
        GameObject locatedTer = TerrainObjs[i, j];                // Located Terrain object in the world

        if (locatedTer != null)
        {
            return locatedTer;
        }

        return null;
    }

    public GameObject[,] getTerrainObjects()
    {
        return TerrainObjs;
    }
    
    public GameObject[,] getVegetationObjects()
    {
        return VegetationObjects;
    }
    
    public bool GetWalkable(int i, int j)
    {
        if (i < 0 || i >= xSize || j < 0 || j >= ySize)
            return false;
        
        if ((TerrainArray[i, j] == types["grass"] ||
             TerrainArray[i, j] == types["rock"]) && 
            VegetationArray[i, j] != vegTypes["tree"])
        {
            return true;
        }

        return false;
    }


    
    // =========================
}


//public GameObject getObjectFromTerrain(int i, int j)
//{
//    GameObject locatedVeg = VegetationObjects[i, j];          // Located Vegetation object on the surface of the terrain
//    GameObject locatedTer = TerrainObjs[i, j];                // Located Terrain object in the world

//    if (locatedTer != null)
//    {
//        if (locatedVeg != null)
//        {
//            return locatedVeg;
//        }

//        return locatedTer;
//    }

//    return null;
//}
