using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : Singleton<MapGenerator>
{
    public enum TileType
    {
        empty = 0,
        floor = 1,
        wall,
        door
    }

    [SerializeField]
    public int MapWidth { get; private set; } = 50;

    [SerializeField]
    public int MapHeight { get; private set; } = 50;

    struct index
    {
        public int x, y;
        public index(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [SerializeField]
    public Vector3 position { get; private set; } = Vector3.zero;

    [Header("Cave Map")]
    public int cave_wall_chance = 45;
    public int iteration_num_total = 7;
    public int iteration_num_round_1 = 4;

    [Header("Obstacles")]
    public int Obs_num;
    private int[,] Obs_map;
    private float noiseOffset;
    [SerializeField] private GameObject[] lowLandObstacles;
    [SerializeField] private GameObject[] midLandObstacles;
    [SerializeField] private GameObject[] highLandObstacles;
    [SerializeField] private GameObject[] lowWaterObstacles;
    [SerializeField] private GameObject[] midWaterObstacles;
    [SerializeField] private GameObject[] highWaterObstacles;

    public void CreateMap(Vector2Int mapSize)
    {
        MapWidth = mapSize.x;
        MapHeight = mapSize.y;
        Map = new TileType[MapWidth, MapHeight];
        MainIsland = new bool[MapWidth, MapHeight];
        Obs_map = new int[MapWidth, MapHeight];
        RegenerateCaveMap();
    }


    /* a list of prafabs used in map
     * 1 - Floor
     * 2 - Wall
     * 3 - Door
     * 4 - Empty
    */
    public GameObject[] elements;

    [Tooltip("The size of tiles created for the map.")]
    public int TileSize { get; private set; } = 1;

    public TileType[,] Map { get; private set; }
    private GameObject grid;
    public bool[,] MainIsland { get; private set; }
    private bool[,] _obsIsland;
    private bool islandFilled = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    private void RegenerateCaveMap()
    {
        clearMap();
        generateCaveMap();
        mapLoad();
    }

    public bool[,] GetBinaryTilemap()
    {
        bool[,] tilemap = new bool[MapWidth, MapHeight];
        if (Map == null)
        {
            return tilemap;
        }

        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                // TODO: Remove wall from this when there is always a non-water pathway.
                if ((Map[i, j] == TileType.floor || Map[i, j] == TileType.door || Map[i, j] == TileType.empty || Map[i, j] == TileType.wall) && Obs_map[i, j] == 0)
                {
                    tilemap[i, j] = true;
                }
                else
                {
                    tilemap[i, j] = false;
                }
            }
        }

        return tilemap;
    }

    public bool isOnMainIsland(int x, int y)
    {
        if (!islandFilled)
        {
            findMainIsland();
            findMainIslandObs();
            islandFilled = true;
        }

        return MainIsland[x, y];// && _obsIsland[x, y];
    }

    private void generateCaveMap()
    {
        // init the map
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Random.Range(0, 100) < cave_wall_chance)
                {
                    Map[i, j] = TileType.wall;
                }
                else
                {
                    Map[i, j] = TileType.floor;
                }
            }
        }

        for (int c = 0; c < iteration_num_total; c++)
        {
            caveIterate(c);
        }

        generateObs();
    }

    private void mapLoad()
    {
        grid = new GameObject();
        grid.name = "grid";
        grid.transform.position = position;

        float x = position.x, y = position.y, z = position.z;

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {

                switch (Map[i, j])
                {
                    case TileType.floor:
                        Instantiate(elements[0], new Vector3(x, y, z), Quaternion.identity, grid.transform);
                        break;
                    case TileType.wall:
                        Instantiate(elements[1], new Vector3(x, y, z), Quaternion.identity, grid.transform);
                        break;
                    case TileType.door:
                        Instantiate(elements[2], new Vector3(x, y, z), Quaternion.identity, grid.transform);
                        break;
                    case TileType.empty:
                        Instantiate(elements[3], new Vector3(x, y, z), Quaternion.identity, grid.transform);
                        break;
                }
                z += TileSize;
            }
            z = position.z;
            x += TileSize;
        }
        obsLoad();
    }

    private void generateObs()
    {
        noiseOffset = Random.Range(0, 1);

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                float x = noiseOffset + (float)i / MapWidth * 10;
                float y = noiseOffset + (float)j / MapHeight * 10;
                float v = Mathf.PerlinNoise(x, y);

                if (v > 0.88)
                {
                    Obs_map[i, j] = 3;
                }
                else if (v > 0.76)
                {
                    Obs_map[i, j] = 2;
                }
                else if (v > 0.64)
                {
                    Obs_map[i, j] = 1;
                }
                else
                {
                    Obs_map[i, j] = 0;
                }
            }
        }
    }

    private void obsLoad()
    {
        float x = position.x, y = position.y, z = position.z;

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Obs_map[i, j] == 1)
                {
                    if (Map[i, j] == TileType.wall)
                    {
                        Instantiate(GetRandomPrefab(lowWaterObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                    else
                    {
                        Instantiate(GetRandomPrefab(lowLandObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                }
                else if (Obs_map[i, j] == 2)
                {
                    if (Map[i, j] == TileType.wall)
                    {
                        Instantiate(GetRandomPrefab(midWaterObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                    else
                    {
                        Instantiate(GetRandomPrefab(midLandObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                }
                else if (Obs_map[i, j] == 3)
                {
                    if (Map[i, j] == TileType.wall)
                    {
                        Instantiate(GetRandomPrefab(highWaterObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                    else
                    {
                        Instantiate(GetRandomPrefab(highLandObstacles), new Vector3(x, y, z),
                            Quaternion.identity, grid.transform);
                    }
                }
                z += TileSize;
            }
            z = position.z;
            x += TileSize;
        }
    }

    private GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs == null)
        {
            return null;
        }
        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }

    #region Help Funcs

    private void clearMap()
    {
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                Map[i, j] = 0;
            }
        }
        Destroy(grid);
        islandFilled = false;
    }

    private int checkNeighborFloor(int x, int y, int range)
    {
        int res = 0;
        for (int i = x - range; i <= x + range; i++)
        {
            if (i > 0 && i < MapWidth)
            {
                for (int j = y - range; j <= y + range; j++)
                {
                    if (j > 0 && j < MapHeight)
                    {
                        if (Map[i, j] == TileType.floor) { res++; }
                    }
                }
            }
        }
        return res;
    }

    private void caveIterate(int c)
    {
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (c <= iteration_num_round_1)
                {
                    int count_1 = checkNeighborFloor(i, j, 1);
                    int count_2 = checkNeighborFloor(i, j, 2);
                    if (count_1 >= 5 || count_2 <= 2)
                    {
                        Map[i, j] = TileType.floor;
                    }
                    else
                    {
                        Map[i, j] = TileType.wall;
                    }
                }
                else
                {
                    int count = checkNeighborFloor(i, j, 1);
                    if (count >= 5)
                    {
                        Map[i, j] = TileType.floor;
                    }
                    else
                    {
                        Map[i, j] = TileType.wall;
                    }
                }
            }
        }
    }

    private void findMainIsland()
    {
        int maxSize = 0;
        int maxX = 0, maxY = 0;

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (isOpen(i, j) && MainIsland[i, j] == false)
                {
                    int size = 0;
                    dfs(i, j, ref size);
                    if (size >= maxSize)
                    {
                        maxSize = size;
                        maxX = i;
                        maxY = j;
                    }
                }
            }
        }

        // reset _mainIsland
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                MainIsland[i, j] = false;
            }
        }

        // mark the main island
        dfs(maxX, maxY, ref maxSize);
    }

    private void findMainIslandObs()
    {
        int maxSize = 0;
        int maxX = 0, maxY = 0;

        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Obs_map[i, j] == 0 && _obsIsland[i, j] == false)
                {
                    int size = 0;
                    dfs_obs(i, j, ref size);
                    if (size >= maxSize)
                    {
                        maxSize = size;
                        maxX = i;
                        maxY = j;
                    }
                }
            }
        }

        // reset _mainIsland
        for (int i = 0; i < MapWidth; i++)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                _obsIsland[i, j] = false;
            }
        }

        // mark the main island
        dfs_obs(maxX, maxY, ref maxSize);
    }

    private void dfs(int i, int j, ref int size)
    {
        size++;
        MainIsland[i, j] = true;

        if (i - 1 >= 0 && isOpen(i - 1, j) && MainIsland[i - 1, j] == false) dfs(i - 1, j, ref size);

        if (i + 1 < MapWidth && isOpen(i + 1, j) && MainIsland[i + 1, j] == false) dfs(i + 1, j, ref size);

        if (j - 1 >= 0 && isOpen(i, j - 1) && MainIsland[i, j - 1] == false) dfs(i, j - 1, ref size);

        if (j + 1 < MapHeight && isOpen(i, j + 1) && MainIsland[i, j + 1] == false) dfs(i, j + 1, ref size);
    }

    private void dfs_obs(int i, int j, ref int size)
    {
        size++;
        _obsIsland[i, j] = true;
        if (i - 1 >= 0 && Obs_map[i - 1, j] == 0 && _obsIsland[i - 1, j] == false) dfs(i - 1, j, ref size);

        if (i + 1 < MapWidth && Obs_map[i + 1, j] == 0 && _obsIsland[i + 1, j] == false) dfs(i + 1, j, ref size);

        if (j - 1 >= 0 && Obs_map[i, j - 1] == 0 && _obsIsland[i, j - 1] == false) dfs(i, j - 1, ref size);

        if (j + 1 < MapHeight && Obs_map[i, j + 1] == 0 && _obsIsland[i, j + 1] == false) dfs(i, j + 1, ref size);
    }

    private bool isOpen(int i, int j)
    {
        return Map[i, j] == TileType.floor || Map[i, j] == TileType.door || Map[i, j] == TileType.empty;
    }
    #endregion

}
