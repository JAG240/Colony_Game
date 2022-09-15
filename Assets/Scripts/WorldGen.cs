using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour
{
    #region Prefabs
    [Header("Prefabs")]
    [SerializeField] private Tile[] groundTiles;
    [SerializeField] private Tile[] stoneTiles;
    [SerializeField] private Tile[] treeTiles;
    [SerializeField] private Tile[] rockTiles;
    [SerializeField] private Tile[] storageTiles;
    [SerializeField] private GameObject tree;
    [SerializeField] private GameObject rock;
    #endregion

    #region Generation Settings 
    [Header("Generation Settings")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private bool useSavedSeeds;
    [SerializeField] private string stringSeed;
    [SerializeField] private int seed;
    #endregion

    #region Assigned Fields
    [Header("Assigned Fields")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private inputManagerScript inputManagerScript;

    public Tilemap ground;
    public Tilemap stones;
    public Tilemap trees;
    public Tilemap rocks;
    public Tilemap storage;
    private Vector2Int startPos;
    private bool[,] spotAvailability;
    private int sizeScaleX = 1;
    private int sizeScaleY = 1;
    private SeedsManager seedsManager;
    private Color groundColor;
    #endregion

    private void Awake()
    {
        //Sets the starting position
        startPos = new Vector2Int(-(size.x / 2), -(size.y / 2));

        //creating all needed tilemaps 
        ground = CreateTilemap("Ground", "Layer 1", new Vector3(0f, 0f, 0f), 0);
        stones = CreateTilemap("Stones", "Layer 2", new Vector3(0f, 0f, 0f), 0);
        trees = CreateTilemap("Trees", "Layer 2", new Vector3(0f, 0f, 0f), 1);
        rocks = CreateTilemap("Rocks", "Layer 2", new Vector3(0f, -0.2f, 0f), 0);
        storage = CreateTilemap("Storage", "Layer 2", new Vector3(0f, 0f, 0f), 0);

        //generates tehe ground, creates spot availabity array and disables spawning on the center of the map
        GenGround();
        spotAvailability = new bool[size.x, size.y]; //Defaults to false so, true is not available and false is.
        DisableGenOnSpawnArea(2, 1);
        //SetStorage();

        //find seed manager and set the seed
        seedsManager = GameObject.Find("Reader").GetComponent<SeedsManager>();
        SetSeed();

        //Sets the color of the ground to a random color (note 255 is color all the way this is a multiplier so, 0.5 * 255 = 127.5 which is the lowest value)
        groundColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
        ground.color = groundColor;

        //sets the size scale used for perlin noise
        sizeScaleX = Mathf.RoundToInt(size.x / 100);
        sizeScaleY = Mathf.RoundToInt(size.y / 100);

        //generates all the things needed
        Generate(stoneTiles, new Vector2Int(3, 6), new Vector2(0.7f, 0.75f), stones);
        Generate(treeTiles, new Vector2Int(2, 4), new Vector2(0.6f, 0.7f), trees);
        Generate(rockTiles, new Vector2Int(7, 15), new Vector2(0.7f, 0.8f), rocks);

        //passes tilemaps to scripts that need a refernce
        PassToScripts();
    }

    void Update()
    {
        
    }

    //sets up tilemaps used for the world 
    private Tilemap CreateTilemap(string name, string layer, Vector3 anchor, int orderInLayer)
    {
        //creates the gameobject with components 
        GameObject gameObj = new GameObject(name);
        Tilemap tilemap = gameObj.AddComponent<Tilemap>();
        TilemapRenderer tileRenderer = gameObj.AddComponent<TilemapRenderer>();

        //sets the settings for the above gameobject
        gameObj.transform.SetParent(transform);
        tilemap.tileAnchor = anchor;
        tileRenderer.sortingLayerName = layer;
        tileRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
        tileRenderer.mode = TilemapRenderer.Mode.Individual;
        tileRenderer.sortingOrder = orderInLayer;

        return tilemap;
    }

    //makes sure that nothing is spawn in the starting area 
    private void DisableGenOnSpawnArea(int x, int y)
    {
        for(int i = -x; i <= x; i++)
        {
            for(int j = -y; j <= y; j++)
            {
                spotAvailability[(size.x/2) - i, (size.y/2) - j] = true;
            }
        }
    }

    //sets up the ground tiles based on size
    private void GenGround()
    {
        Vector3Int[] postions = new Vector3Int[size.x * size.y];
        TileBase[] tileArray = new TileBase[postions.Length];
        
        for(int i = 0; i < postions.Length; i++)
        {
            postions[i] = new Vector3Int(i % size.x + startPos.x, i / size.y + startPos.y, 0);
            tileArray[i] = groundTiles[Random.Range(0, 5)];
        }

        ground.SetTiles(postions, tileArray);
    }

    //Genertes all types of gameobjects 
    private void Generate(GameObject obj, Vector2Int scale, Vector2 thresholdRange)
    {
        //offsets the perlin noise map 
        float offsetX = Random.Range(10, 999999);
        float offsetY = Random.Range(10, 999999);

        //sets a random scale to the perlin noise map 
        int scaleX = Random.Range(scale.x * sizeScaleX, scale.y * sizeScaleX);
        int scaleY = Random.Range(scale.x * sizeScaleY, scale.y *sizeScaleY);

        //randomly selects a threshold for perlin noise. Threshold meaning the range of numbers that will generate. 
        float threshold = Random.Range(thresholdRange.x, thresholdRange.y);

        for (int x = startPos.x; x < (startPos.x + size.x); x++)
        {
            for (int y = startPos.y; y < (startPos.y + size.y); y++)
            {
                float perlinX = (float)x / size.x * scaleX + offsetX;
                float perlinY = (float)y / size.y * scaleY + offsetY;
                float result = Mathf.PerlinNoise(perlinX, perlinY);

                if (result > threshold && !spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)])
                {
                    Instantiate(obj, new Vector3(x, y, 0f), Quaternion.identity);
                    spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)] = true;
                }
            }
        }
    }

    //Generates all types of tiles
    private void Generate(Tile[] tiles, Vector2Int scale, Vector2 thresholdRange, Tilemap map)
    {
        float offsetX = Random.Range(10, 999999);
        float offsetY = Random.Range(10, 999999);
        int scaleX = Random.Range(scale.x *sizeScaleX, scale.y * sizeScaleX);
        int scaleY = Random.Range(scale.x * sizeScaleY, scale.y * sizeScaleY);
        float threshold = Random.Range(thresholdRange.x, thresholdRange.y);

        for (int x = startPos.x; x < (startPos.x + size.x); x++)
        {
            for (int y = startPos.y; y < (startPos.y + size.y); y++)
            {
                float perlinX = (float)x / size.x * scaleX + offsetX;
                float perlinY = (float)y / size.y * scaleY + offsetY;
                float result = Mathf.PerlinNoise(perlinX, perlinY);

                if (result > threshold && !spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)])
                {
                    map.SetTile(new Vector3Int(x, y, 0), tiles[Random.Range(0,tiles.Length)]);
                    spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)] = true;
                }
            }
        }
    }

    //Set seed sets the seed
    private void SetSeed()
    {
        //using only seeds that have been previously saved
        if(useSavedSeeds)
        {
            seed = seedsManager.GetSeed();
            Random.InitState(seed);
        }
        //use the string seed if there is one 
        else if(stringSeed != "")
        {
            seed = stringSeed.GetHashCode();
            Random.InitState(seed);
        }
        //if there is neither make a completely random seed 
        else
        {
            seed = Random.Range(0, int.MaxValue);
            Random.InitState(seed);
        }
    }

    //Save seed saves the seed to a txt that can be read again 
    public void SaveSeed()
    {
        seedsManager.SaveSeed(seed);
    }

    //Uses regular x and y and converts it to usable info from the spotAvailabilty matrix
    public bool GetSpotAvailabilty(int x, int y)
    {
        return spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)];
    }

    //Sets the availabilty of spots with regular x and y
    public void SetSpotAvailabilty(int x, int y, bool set)
    {
        spotAvailability[x + Mathf.Abs(startPos.x), y + Mathf.Abs(startPos.y)] = set;
    }

    //passes tilemaps to scripts that require them
    private void PassToScripts()
    {
        gridManager.ground = ground;
        gridManager.stones = stones;
        inputManagerScript.stones = stones;
        inputManagerScript.trees = trees;
        inputManagerScript.rocks = rocks;
    }

    //Temp method to set storage area down 
    private void SetStorage()
    {
        storage.SetTile(new Vector3Int(0, 0, 0), storageTiles[0]);
    }
}
