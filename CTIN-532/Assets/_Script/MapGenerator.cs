using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : Singleton<MapGenerator>
{
    enum TileType
    {
        empty = 0,
        floor = 1,
        wall,
        door
    }

    struct index
    {
        public int x, y;
        public index(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    #region Fields
    // height width of the map
    [Header("General")]
    public Vector3 position = Vector3.zero;
    public int map_height = 50, map_width = 50;

    [Header("Pathos Map")]
    public int room_height_min = 5;
    public int room_height_max = 10;
    public int room_width_min = 5, room_width_max = 10;
    public int corridor_length = 10;
    public int corridor_width = 3;
    public int num_of_elements = 10;
    public int chance_of_room = 40;

    private int numTries = 3000;
    private List<index> indexOfWall;


    [Header("Cave Map")]
    public int cave_wall_chance = 45;
    public int iteration_num_total = 7;
    public int iteration_num_round_1 = 4;

    /* a list of prafabs used in map
     * 1 - Floor
     * 2 - Wall
     * 3 - Door
     * 4 - Empty
    */
    public GameObject[] elements;

    [Tooltip("The size of tiles created for the map.")]
    public int TileSize { get; private set; } = 10;

    private TileType[,] _map;
    private GameObject grid;
    private bool[,] _mainIsland;
    private bool islandFilled = false;

    [Header("Map Generation Input Response")]
    [Tooltip("Whether or not this map generator will responsd to keyboard input for generating maps.")]
    public bool RespondsToInputSystem;

    // main island debug
    private GameObject highlightGrid;

    #endregion

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        RespondsToInputSystem = true;
        _map = new TileType[map_width, map_height];
        _mainIsland = new bool[map_width,map_height];
        indexOfWall = new List<index>();
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        Debug.Log("Press P to Generate Pathos Type Map");
        Debug.Log("Press C to Generate Cave Type Map");
        Debug.Log("Press R to Clear the Board");
    }

    private void Update()
    {
        if (RespondsToInputSystem)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                RegenerateRoomMap();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                RegenerateCaveMap();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                clearMap();
            }

            // mainIsland Debug
            if (Input.GetKeyDown(KeyCode.H))
            {
                isOnMainIsland(0, 0);
                MainislandLoad();
            }


            if (Input.GetKeyDown(KeyCode.J))
            {
                MainIslandDestory();
            }

        }
    }

    public void RegenerateRoomMap()
    {
        clearMap();
        generateRoomMap();
        mapLoad();
    }

    public void RegenerateCaveMap()
    {

        clearMap();
        generateCaveMap();
        mapLoad();
    }

    /// <summary>
    /// This function exposed the underlying map data as a 2D array of integers.
    /// </summary>
    /// <returns>Returns a 2D array of bools representing the map data where true means the tile is blocked by an obstacle, wall, or not part of the map.</returns>
    public bool[,] GetBinaryTilemap()
    {
        bool[,] tilemap = new bool[map_width, map_height];
        if (_map == null)
        {
            return tilemap;
        }

        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                if(_map[i, j] == TileType.floor || _map[i, j] == TileType.door)
                {
                    tilemap[i, j] = true;
                } else
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
            islandFilled = true;
        }

        return _mainIsland[x, y];
    }

    private void generateRoomMap()
    {
        // For Debug
        int room = 0, cor = 0;

        // add first Room
        int roomH = Random.Range(room_height_min, room_height_max);
        int roomW = Random.Range(room_width_min, room_width_max);

        // size after add walls
        roomH += 2;
        roomW += 2;

        int x = (map_width - roomW) / 2;
        int y = (map_height - roomH) / 2;

        for (int i = x; i < x + roomW; i++)
        {
            for (int j = y; j < y + roomH; j++)
            {
                if (i == x || j == y || i == x + roomW - 1 || j == y + roomH - 1)
                {
                    _map[i, j] = TileType.wall;
                    indexOfWall.Add(new index(i, j));
                }
                else
                {
                    _map[i, j] = TileType.floor;
                }
            }
        }

        int numE = 0;

        for (int i = 1; i < numTries; i++)
        {
            int chances = Random.Range(0, 100);
            // Find Open Direction
            index p = indexOfWall[Random.Range(0, indexOfWall.Count)];
            // 0 - left, 1 - up, 2 - right, 3 - down
            int direction = 5;

            int tryCounts = 0;
            while (tryCounts < 100)
            {
                if (p.x - 1 > 0 && _map[p.x - 1, p.y] == TileType.empty)
                {
                    direction = 0;
                    break;
                }
                else if (p.y + 1 < map_height && _map[p.x, p.y + 1] == TileType.empty)
                {
                    direction = 1;
                    break;
                }
                else if (p.x + 1 < map_width && _map[p.x + 1, p.y] == TileType.empty)
                {
                    direction = 2;
                    break;
                }
                else if (p.y - 1 < 0 && _map[p.x, p.y - 1] == TileType.empty)
                {
                    direction = 3;
                    break;
                }
                p = indexOfWall[Random.Range(0, indexOfWall.Count)];
                tryCounts++;
            }

            // No enough space
            if (direction == 5)
            {
                Debug.Log("No open wall");
                return;
            }

            if (chances <= chance_of_room)
            {
                room++;
                if (addRoom(p, direction))
                {
                    numE++;
                }
            }
            else
            {
                cor++;
                if (addCorridor(p, direction))
                {
                    numE++;
                }
            }

            if (numE == num_of_elements) { break; }

        }
        Debug.Log("Room: " + room + " //Cor: " + cor);
    }

    public void generateCaveMap()
    {
        // init the map
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                if (Random.Range(0, 100) < cave_wall_chance)
                {
                    _map[i, j] = TileType.wall;
                }
                else
                {
                    _map[i, j] = TileType.floor;
                }
            }
        }

        for (int c = 0; c < iteration_num_total; c++)
        {
            caveIterate(c);
        }
    }

    public void mapLoad()
    {
        grid = new GameObject();
        grid.name = "grid";
        grid.transform.position = position;

        float x = position.x, y = position.y, z = position.z;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {

                switch (_map[i, j])
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
    }

    #region Help Funcs
    
    private bool addRoom(index pos, int direction)
    {
        int roomH = Random.Range(room_height_min, room_height_max);
        int roomW = Random.Range(room_width_min, room_width_max);

        // check empty space
        int xStart = 0, xEnd = 0, yStart = 0, yEnd = 0, temp;
        // 0 - left, 1 - up, 2 - right, 3 - down
        switch (direction)
        {
            case 0:
                xStart = pos.x - roomW - 1;
                xEnd = pos.x;
                temp = Random.Range(0, roomH);
                yStart = pos.y - temp - 1;
                yEnd = pos.y + (roomH - temp);
                break;
            case 1:
                temp = Random.Range(0, roomW);
                xStart = pos.x - temp - 1;
                xEnd = pos.x + (roomW - temp);
                yStart = pos.y;
                yEnd = pos.y + roomH + 1;
                break;
            case 2:
                xStart = pos.x;
                xEnd = pos.x + roomW + 1;
                temp = Random.Range(0, roomH);
                yStart = pos.y - temp - 1;
                yEnd = pos.y + (roomH - temp);
                break;
            case 4:
                temp = Random.Range(0, roomW);
                xStart = pos.x - temp - 1;
                xEnd = pos.x + (roomW - temp);
                yStart = pos.y - roomH - 1;
                yEnd = pos.y;
                break;
        }
        // check out of bound error
        if (xStart < 0 || xEnd >= map_width || yStart < 0 || yEnd >= map_height)
            return false;

        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                if (i == xStart || j == yStart || i == xEnd || j == yEnd)
                {
                    if (_map[i, j] != TileType.empty &&
                        _map[i, j] != TileType.wall &&
                        _map[i, j] != TileType.door)
                        return false;
                }
                else
                {
                    if (_map[i, j] != TileType.empty)
                        return false;
                }
            }
        }

        // fill the room 
        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                if (i == xStart || j == yStart || i == xEnd || j == yEnd)
                {
                    _map[i, j] = TileType.wall;
                    indexOfWall.Add(new index(i, j));
                }
                else
                {
                    _map[i, j] = TileType.floor;
                }
            }
        }

        _map[pos.x, pos.y] = TileType.door;

        return true;
    }

    private bool addCorridor(index pos, int direction)
    {
        int length = Random.Range(3, corridor_length);

        // check empty space
        int xStart = 0, xEnd = 0, yStart = 0, yEnd = 0;
        // 0 - left, 1 - up, 2 - right, 3 - down
        switch (direction)
        {
            case 0:
                xStart = pos.x - length - 1;
                xEnd = pos.x;
                yStart = pos.y - corridor_width / 2 - 1;
                yEnd = pos.y + (corridor_width - corridor_width / 2);
                break;
            case 1:
                xStart = pos.x - corridor_width / 2 - 1;
                xEnd = pos.x + (corridor_width - corridor_width / 2);
                yStart = pos.y;
                yEnd = pos.y + length + 1;
                break;
            case 2:
                xStart = pos.x;
                xEnd = pos.x + length + 1;
                yStart = pos.y - corridor_width / 2 - 1;
                yEnd = pos.y + (corridor_width - corridor_width / 2);
                break;
            case 4:
                xStart = pos.x - corridor_width / 2 - 1;
                xEnd = pos.x + (corridor_width - corridor_width / 2);
                yStart = pos.y - length - 1;
                yEnd = pos.y;
                break;
        }

        if (xStart < 0 || xEnd >= map_width || yStart < 0 || yEnd >= map_height)
            return false;

        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                if (i == xStart || j == yStart || i == xEnd || j == yEnd)
                {
                    if (_map[i, j] != TileType.empty &&
                        _map[i, j] != TileType.wall &&
                        _map[i, j] != TileType.door)
                        return false;
                }
                else
                {
                    if (_map[i, j] != TileType.empty)
                        return false;
                }
            }
        }

        // fill the room 
        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                if (i == xStart || j == yStart || i == xEnd || j == yEnd)
                {
                    _map[i, j] = TileType.wall;
                    indexOfWall.Add(new index(i, j));
                }
                else
                {
                    _map[i, j] = TileType.floor;
                }
            }
        }

        _map[pos.x, pos.y] = TileType.door;

        return true;

    }

    private void clearMap()
    {
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                _map[i, j] = 0;
            }
        }
        Destroy(grid);
        indexOfWall.Clear();
        islandFilled = false;
    }

    private int checkNeighborFloor(int x, int y, int range)
    {
        int res = 0;
        for (int i = x - range; i <= x + range; i++)
        {
            if (i > 0 && i < map_width)
            {
                for (int j = y - range; j <= y + range; j++)
                {
                    if (j > 0 && j < map_height)
                    {
                        if (_map[i, j] == TileType.floor) { res++; }
                    }
                }
            }
        }
        return res;
    }

    private void caveIterate(int c)
    {
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                if (c <= iteration_num_round_1)
                {
                    int count_1 = checkNeighborFloor(i, j, 1);
                    int count_2 = checkNeighborFloor(i, j, 2);
                    if (count_1 >= 5 || count_2 <= 2)
                    {
                        _map[i, j] = TileType.floor;
                    }
                    else
                    {
                        _map[i, j] = TileType.wall;
                    }
                }
                else
                {
                    int count = checkNeighborFloor(i, j, 1);
                    if (count >= 5)
                    {
                        _map[i, j] = TileType.floor;
                    }
                    else
                    {
                        _map[i, j] = TileType.wall;
                    }
                }
            }
        }
    }

    private void findMainIsland()
    {
        int maxSize = 0;
        int maxX = 0, maxY = 0;

        for(int i = 0; i<map_width; i++)
        {
            for(int j = 0; j<map_height; j++)
            {
                if (isOpen(i,j) && _mainIsland[i,j] == false)
                {
                    int size = 0;
                    dfs(i, j,ref size);
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
        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {
                _mainIsland[i, j] = false;
            }
        }

        // mark the main island
        dfs(maxX, maxY, ref maxSize);
    }

    private void dfs(int i, int j, ref int size)
    {
        size++;
        _mainIsland[i, j] = true;

        if (i - 1 >= 0 && isOpen(i - 1, j) && _mainIsland[i - 1, j] == false) dfs(i - 1, j,ref size);
        
        if (i + 1 < map_width && isOpen(i+1,j) && _mainIsland[i + 1, j] == false) dfs(i + 1, j, ref size);

        if (j - 1 >= 0 && isOpen(i, j-1) && _mainIsland[i , j - 1] == false) dfs(i, j - 1, ref size);
        
        if (j + 1 < map_height && isOpen(i, j+1) && _mainIsland[i, j + 1] == false) dfs(i, j + 1, ref size);
    }

    private bool isOpen(int i, int j)
    {
        return _map[i, j] == TileType.floor || _map[i, j] == TileType.door;
    }
    #endregion

    #region Debug Tools
    private void MainislandLoad()
    {
        if (highlightGrid != null) return;
        highlightGrid = new GameObject();
        float x = position.x, y = position.y+1, z = position.z;

        for (int i = 0; i < map_width; i++)
        {
            for (int j = 0; j < map_height; j++)
            {

                switch (_mainIsland[i, j])
                {
                    case true:
                        Instantiate(elements[4], new Vector3(x, y, z), Quaternion.identity, highlightGrid.transform);
                        break;
                    case false:
                        Instantiate(elements[5], new Vector3(x, y, z), Quaternion.identity, highlightGrid.transform);
                        break;
                }
                z += TileSize;
            }
            z = position.z;
            x += TileSize;
        }
    }

    private void MainIslandDestory() { Destroy(highlightGrid); }

    public void PrintMapLocation(int x, int y)
    {
        Debug.Log("Generated map location (" + x + ", " + y + "): " + _map[x, y]);
    }

    private void printMap()
    {

        for (int i = 0; i < map_width; i++)
        {
            string res = "";
            for (int j = 0; j < map_height; j++)
            {
                switch (_map[i, j])
                {
                    case TileType.empty:
                        res += " ";
                        break;
                    case TileType.floor:
                        res += "F";
                        break;
                    case TileType.wall:
                        res += "#";
                        break;
                    case TileType.door:
                        res += "D";
                        break;
                }
            }
            res += "\n";
            Debug.Log(res);
        }
    }

    #endregion
}
