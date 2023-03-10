using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static MapNodeController;
using Random = UnityEngine.Random;

/// <summary>
/// This class creates the game's entities procedurally which sets up a playable level within the game.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    /// <summary>
    /// A prefab for the player HQs.
    /// </summary>
    public GameObject HeadquartersPrefab;

    /// <summary>
    /// An offset for any level objects' y-position to move them above the map layer.
    /// </summary>
    public float LevelmapWorldSpaceYPosition = 1;

    /// <summary>
    /// The minimum number of tiles to skip when searching for positions to place new level objects.
    /// </summary>
    [Range(0, 255)]
    public int MinimumSearchPositionTolerance = 8;

    /// <summary>
    /// The maximum number of tiles to skip when searching for positions to place new level objects.
    /// </summary>
    [Range(0, 255)]
    public int MaximumSearchPositionTolerance = 64;

    /// <summary>
    /// The chance to skip a searched tile when placing new level objects betweenthe min/max range.
    /// </summary>
    [Range(0, 1)]
    public float SearchPositionToleranceChance = 0.05f;

    /// <summary>
    /// This keeps track of how many tiles searched have been passed on when determining the position of level objects such as the players' HQs.
    /// </summary>
    private int searchPositionsPassedOn;

    /// <summary>
    /// This is the game object which will serve as the parent node to all game objects in the level.
    /// </summary>
    private GameObject levelGameObject;

    /// <summary>
    /// This is a reference to a map generator which will create the underlying data structure for map data.
    /// The level will place game entities based on the map data.
    /// </summary>
    private MapGenerator mapGenerator;

    /// <summary>
    /// This 2D array is the set of map data valuesfrom the map generator for placing other level objects such as units and cities.
    /// </summary>
    private bool[,] tilemap;

    /// <summary>
    /// This 2D array stores the positions of level entites on top of the map data such as player headquarters.
    /// </summary>
    private int[,] levelmap;

    private SelectedObjects humanPlayerController;


    [Header("Obstacles")]
    public int Obs_num;
    public int Obs_num_per_flood;
    public GameObject Obs_prefab_land;
    public GameObject Obs_prefab_water;
    private List<Tuple<int, int>> node_pos;
    private List<Tuple<int, int>> Obs_pos;
    private int[,] Obs_map;


    enum LevelTileType
    {
        invalid = -1,
        open = 0,
        headquarters = 1
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        tilemap = null;
        levelmap = null;
        levelGameObject = new GameObject("The Level");
        if (MinimumSearchPositionTolerance > MaximumSearchPositionTolerance)
        {
            MinimumSearchPositionTolerance = MaximumSearchPositionTolerance;
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Find the human player selection controller:
        SelectedObjects[] controllers = FindObjectsOfType<SelectedObjects>();
        foreach (var controller in controllers)
        {
            if (controller.Owner == MapNodeController.Player.Human)
            {
                humanPlayerController = controller;
            }
        }

        // Search the scene for a reference to the map generator:
        mapGenerator = FindObjectOfType<MapGenerator>();

        // Turn off the map generator's response to input so that the level generator can clear its entities before a new map is generated and add them back in afterwards.
        if (mapGenerator != null)
        {
            mapGenerator.RespondsToInputSystem = false;
        }
        else
        {
            Debug.LogWarning("The level generator was unable to find a reference to the map generator.");
        }

        node_pos = new List<Tuple<int, int>>();
        Obs_map = new int[mapGenerator.map_width, mapGenerator.map_height];
        Obs_pos = new List<Tuple<int, int>>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (mapGenerator != null)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                mapGenerator.RegenerateRoomMap();
                tilemap = mapGenerator.GetBinaryTilemap();
                regenerateLevel();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                regenerateCaveMap();
            }
        }

        if (tilemap != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                regenerateLevel();
            }
        }
        else
        {
            // Generate a cave map by default:
            regenerateCaveMap();
        }
    }

    private void regenerateCaveMap()
    {
        mapGenerator.RegenerateCaveMap();
        tilemap = mapGenerator.GetBinaryTilemap();
        regenerateLevel();
    }

    /// <summary>
    /// Clears all of the current level's entities and generates a new one based on the underlying map data.
    /// </summary>
    private void regenerateLevel()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return;
        }
        else
        {
            // Recreate the levelmap and destroy any current gameobjects:
            levelmap = new int[tilemap.GetLength(0), tilemap.GetLength(1)];
            foreach (Transform child in levelGameObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Block any cells in the level map which are inpassable in the tilemap:
            for (int i = 0; i < tilemap.GetLength(0); i++)
            {
                for (int j = 0; j < tilemap.GetLength(1); j++)
                {
                    if (tilemap[i, j])
                    {
                        levelmap[i, j] = 0;
                    }
                    else
                    {
                        levelmap[i, j] = -1;
                    }
                }
            }
        }

        Debug.Log("Regenerating the level.");
        //placePlayerHeadquarters();
        placeNodes();
        //placeObstacle();
    }

    /// <summary>
    /// Clears any current player headquarter's and places two new ones in two different random corners of the map.
    /// The HQs are always placed on passable tiles of the map (assuming these exist).
    /// Corners without passable tiles are searched for the nearest passable tile to place an HQ on.
    /// </summary>
    private void placePlayerHeadquarters()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return;
        }

        Debug.Log("Placing two player headquarters within the level.");

        // Each player needs to be in a different corner of the map:
        int playerOneCornerIndex = (int)math.floor(Random.Range(0, 4));
        int playerTwoCornerIndex;
        switch (playerOneCornerIndex)
        {
            case 0:
                playerTwoCornerIndex = 2;
                break;
            case 1:
                playerTwoCornerIndex = 3;
                break;
            case 2:
                playerTwoCornerIndex = 0;
                break;
            case 3:
            default:
                playerTwoCornerIndex = 1;
                break;
        }
        Debug.Log("Placing player one's HQ in corner: " + playerOneCornerIndex);
        Debug.Log("Placing player two's HQ in corner: " + playerTwoCornerIndex);
        Tuple<int, int> playerOneCornerPosition = convertCornerIndexToXYTuple(playerOneCornerIndex);
        Tuple<int, int> playerTwoCornerPosition = convertCornerIndexToXYTuple(playerTwoCornerIndex);
        if (playerOneCornerPosition == null || playerTwoCornerPosition == null)
        {
            Debug.LogError("Failed to select corners for player positions.");
            return;
        }

        // The corner tiles might not be passable, so a search must execute to find the nearest passable tile.
        Tuple<int, int> playerOneHqTilemapPosition = findNearestPassableTileBfs(playerOneCornerPosition);
        Tuple<int, int> playerTwoHqTilemapPosition = findNearestPassableTileBfs(playerTwoCornerPosition);
        if (playerOneHqTilemapPosition == null || playerTwoHqTilemapPosition == null)
        {
            Debug.LogError("Failed to select headquarter positions for the player positions.");
            return;
        }

        Debug.Log("Nearest passable tile for player one's HQ is: " + playerOneHqTilemapPosition);
        Debug.Log("Tilemap (" + playerOneHqTilemapPosition.Item1 + ", " + playerOneHqTilemapPosition.Item2 + ") is: " + tilemap[playerOneHqTilemapPosition.Item1, playerOneHqTilemapPosition.Item2]);
        Debug.Log("Nearest passable tile for player two's HQ is: " + playerTwoHqTilemapPosition);
        Debug.Log("Tilemap (" + playerTwoHqTilemapPosition.Item1 + ", " + playerTwoHqTilemapPosition.Item2 + ") is: " + tilemap[playerTwoHqTilemapPosition.Item1, playerTwoHqTilemapPosition.Item2]);
        if (mapGenerator != null)
        {
            mapGenerator.PrintMapLocation(playerOneHqTilemapPosition.Item1, playerOneHqTilemapPosition.Item2);
            mapGenerator.PrintMapLocation(playerTwoHqTilemapPosition.Item1, playerTwoHqTilemapPosition.Item2);
        }

        if (HeadquartersPrefab != null)
        {
            // Place Player One's HQ:
            placePlayerHQ(playerOneHqTilemapPosition, true);

            // Place Player Two's HQ:
            placePlayerHQ(playerTwoHqTilemapPosition);
        }
        else
        {
            Debug.LogWarning("Headquarters prefab is null.");
        }
    }

    private void placePlayerHQ(Tuple<int, int> tilemapPosition, bool isHumanPlayerHq = false, bool isAiPlayerHq = false)
    {
        Vector3 hqPosition = getLevelmapPositionInWorldSpace(tilemapPosition);
        GameObject playerHq = Instantiate(HeadquartersPrefab, hqPosition, Quaternion.identity, levelGameObject.transform);
        MapNodeController hqController = playerHq.GetComponent<MapNodeController>();
        if (isHumanPlayerHq)
        {
            hqController.SetOwner(Player.Human);
            if (humanPlayerController != null)
            {
                humanPlayerController.SetSelectedMapNode(hqController);
            }
            else
            {
                Debug.LogError("The controller for the human player is null when trying to assign a newly placed HQ map node as the player's selection default.");
            }
        }
        else if (isAiPlayerHq)
        {
            hqController.SetOwner(Player.AI);
        }
        else
        {
            hqController.SetOwner(Player.Neutral);
        }

        playerHq.transform.parent = levelGameObject.transform;
    }

    private Vector3 getLevelmapPositionInWorldSpace(Tuple<int, int> tilemapPosition)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return new Vector3(tilemapPosition.Item1, 0, tilemapPosition.Item2);
        }

        if (mapGenerator == null)
        {
            Debug.LogError("Map generator is null.");
            return new Vector3(tilemapPosition.Item1, 0, tilemapPosition.Item2);
        }

        return new Vector3(tilemapPosition.Item1 * mapGenerator.TileSize + mapGenerator.position.x, LevelmapWorldSpaceYPosition + mapGenerator.position.y, tilemapPosition.Item2 * mapGenerator.TileSize + mapGenerator.position.z);
    }

    /// <summary>
    /// Converts a corner index to an (x, y) tuple of the tilemap.
    /// </summary>
    /// <param name="cornerIndex">An integer [0, 3] representing the four corners of a tilemap.</param>
    /// <returns>An (x,y) pair for the corresponding corner.</returns>
    private Tuple<int, int> convertCornerIndexToXYTuple(int cornerIndex)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return null;
        }

        return cornerIndex switch
        {
            0 => new Tuple<int, int>(0, 0),
            1 => new Tuple<int, int>(tilemap.GetLength(0) - 1, 0),
            2 => new Tuple<int, int>(tilemap.GetLength(0) - 1, tilemap.GetLength(1) - 1),
            _ => new Tuple<int, int>(0, tilemap.GetLength(1) - 1),
        };
    }

    /// <summary>
    /// Given an (x, y) position within the tilemap, this function uses breadth-first search to find the nearest passable tile that does not already contain another level entity.
    /// </summary>
    /// <param name="tilemapSearchStartPostiion">The given tilemap position (x, y) to begin the search from.</param>
    /// <returns>Returns a tilemap position (x,y) of the nearest passable tile to the search start position.</returns>
    private Tuple<int, int> findNearestPassableTileBfs(Tuple<int, int> tilemapSearchStartPostiion)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return null;
        }

        if (levelmap == null)
        {
            Debug.LogError("Levelmap data is null.");
            return null;
        }

        if (tilemap.GetLength(0) != levelmap.GetLength(0) || tilemap.GetLength(1) != levelmap.GetLength(1))
        {
            Debug.LogError("Levelmap dimensions do not match tilemap dimenstions.");
            return null;
        }

        // Reset the number of search positions that have been ignored to allow for some randomness:
        searchPositionsPassedOn = 0;

        // Create a queue to keep track of which tiles to search from next:
        Queue<Tuple<int, int>> searchFrontier = new();

        // Create a data structure to store which tiles of the map have already been visited:
        bool[,] cellsVisited = new bool[tilemap.GetLength(0), tilemap.GetLength(1)];

        // Initialize the BFS by setting the start tile as 'visited' and queueing it on the search frontier:
        cellsVisited[tilemapSearchStartPostiion.Item1, tilemapSearchStartPostiion.Item2] = true;
        searchFrontier.Enqueue(tilemapSearchStartPostiion);

        // Continue with BFS until a passable tile is found:
        while (searchFrontier.Count != 0)
        {
            Tuple<int, int> nextTileToSearch = searchFrontier.Peek();
            int x = nextTileToSearch.Item1;
            int y = nextTileToSearch.Item2;
            searchFrontier.Dequeue();

            // If the tile is passable, then the tile being searched for has been found:
            if (levelmap[x, y] != -1)
            {
                searchPositionsPassedOn++;
                if (searchPositionsPassedOn >= MinimumSearchPositionTolerance)
                {
                    if (searchPositionsPassedOn >= MaximumSearchPositionTolerance || Random.Range(0.0f, 1.0f) < SearchPositionToleranceChance)
                    {
                        Debug.Log("Tiles skipped in search: " + searchPositionsPassedOn);
                        return new Tuple<int, int>(x, y);
                    }
                }
            }

            // Find the four adjacent tiles in the cardinal directions and, if any are valid, queue them for further search:
            List<Tuple<int, int>> adjacentTiles = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(x - 1, y),
                new Tuple<int, int>(x + 1, y),
                new Tuple<int, int>(x, y - 1),
                new Tuple<int, int>(x, y + 1),
            };

            // Before queuing onto the frontier, shuffle the adjacent tiles randomly using Fisher-Yates shuffle:
            System.Random randomNumberGenerator = new();
            int countAwaitingShuffle = adjacentTiles.Count;
            while (countAwaitingShuffle > 1)
            {
                countAwaitingShuffle--;
                int nextIndex = randomNumberGenerator.Next(countAwaitingShuffle + 1);
                Tuple<int, int> value = adjacentTiles[nextIndex];
                adjacentTiles[nextIndex] = adjacentTiles[countAwaitingShuffle];
                adjacentTiles[countAwaitingShuffle] = value;
            }

            // Check if any of the adjacent tiles are valid for continuation of the search: 
            foreach (Tuple<int, int> pair in adjacentTiles)
            {
                if (isCellValidForContinuingSearch(cellsVisited, pair.Item1, pair.Item2))
                {
                    searchFrontier.Enqueue(pair);
                    cellsVisited[pair.Item1, pair.Item2] = true;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether a given cell is a valid cell for searching.
    /// This is base on whether it has already been searched,
    /// whtether it is in the boundaries of the map,
    /// and if anything else in the level already occupies that position.
    /// </summary>
    /// <param name="cellsSearched">The set of cells which have already been searched.</param>
    /// <param name="x">The tilemap x index of the cell to be checked for validity.</param>
    /// <param name="y">The tilemap y index of the cell to be checked for validity.</param>
    /// <returns>Returns true if the cell is valid to be searched, else returns false.</returns>
    private bool isCellValidForContinuingSearch(bool[,] cellsSearched, int x, int y)
    {
        if (levelmap == null)
        {
            Debug.LogError("Levelmap data is null.");
        }

        if (cellsSearched == null)
        {
            Debug.LogError("Cells searched data is null.");
        }

        // To be valid, the cell to check must be within the map boundaries.
        if (x < 0 || x >= levelmap.GetLength(0) || y < 0 || y >= levelmap.GetLength(1))
        {
            return false;
        }

        // To be valid, the cell to check must not have already been visited.
        if (cellsSearched[x, y])
        {
            return false;
        }

        return true;
    }


    private void placeNodes()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return;
        }

        Debug.Log("Placing 5 nodes within the level.");

        Tuple<int, int> CornerNodePosition_1 = convertCornerIndexToXYTuple(0);
        Tuple<int, int> CornerNodePosition_2 = convertCornerIndexToXYTuple(1);
        Tuple<int, int> CornerNodePosition_3 = convertCornerIndexToXYTuple(2);
        Tuple<int, int> CornerNodePosition_4 = convertCornerIndexToXYTuple(3);
        if (CornerNodePosition_1 == null || CornerNodePosition_2 == null || CornerNodePosition_3 == null || CornerNodePosition_4 == null)
        {
            Debug.LogError("Failed to select corners for player positions.");
            return;
        }

        // The corner tiles might not be passable, so a search must execute to find the nearest passable tile.
        Tuple<int, int> nodeTilemapPosition_1 = findNearestPassableTileBfs(CornerNodePosition_1);
        Tuple<int, int> nodeTilemapPosition_2 = findNearestPassableTileBfs(CornerNodePosition_2);
        Tuple<int, int> nodeTilemapPosition_3 = findNearestPassableTileBfs(CornerNodePosition_3);
        Tuple<int, int> nodeTilemapPosition_4 = findNearestPassableTileBfs(CornerNodePosition_4);
        int middleOffset = 5;
        int middlePosx = Random.Range((tilemap.GetLength(0) - 1) / 2 - middleOffset, (tilemap.GetLength(0) - 1) / 2 + middleOffset);
        int middlePosy = Random.Range((tilemap.GetLength(1) - 1) / 2 - middleOffset, (tilemap.GetLength(1) - 1) / 2 + middleOffset);
        Tuple<int, int> nodeTilemapPosition_5 = findNearestPassableTileBfs(
            new Tuple<int, int>(middlePosx, middlePosy)); // Middle Nodes

        if (nodeTilemapPosition_1 == null || nodeTilemapPosition_2 == null
            || nodeTilemapPosition_3 == null || nodeTilemapPosition_4 == null || nodeTilemapPosition_5 == null)
        {
            Debug.LogError("Failed to select headquarter positions for the player positions.");
            return;
        }

        if (HeadquartersPrefab != null)
        {
            placePlayerHQ(nodeTilemapPosition_1, isHumanPlayerHq: true);
            placePlayerHQ(nodeTilemapPosition_2);
            placePlayerHQ(nodeTilemapPosition_3, isAiPlayerHq: true);
            placePlayerHQ(nodeTilemapPosition_4);
            placePlayerHQ(nodeTilemapPosition_5);

            node_pos.Add(nodeTilemapPosition_1);
            node_pos.Add(nodeTilemapPosition_2);
            node_pos.Add(nodeTilemapPosition_3);
            node_pos.Add(nodeTilemapPosition_4);
            node_pos.Add(nodeTilemapPosition_5);
        }
        else
        {
            Debug.LogWarning("Headquarters prefab is null.");
        }

        CameraControl c = FindObjectOfType<CameraControl>();
        c.setFocus(getLevelmapPositionInWorldSpace(nodeTilemapPosition_1));
    }

    private void placeObstacle()
    {
        int try_num = 0;
        int num_left = Obs_num;
        
        while(num_left > 0 && try_num < 50)
        {
            for (int a = 0; a < node_pos.Count; a++)
            {
                Obs_map[node_pos[a].Item1, node_pos[a].Item2] = -2;
            }
            for (int a = 0; a < Obs_pos.Count; a++)
            {
                Obs_map[Obs_pos[a].Item1, Obs_pos[a].Item2] = -1;
            }
            //int[,] cur_obs_map = Obs_map;
            List<Tuple<int, int>> cur_obs_pos = new List<Tuple<int, int>>();
            int i = Obs_num_per_flood;
            if (num_left < Obs_num_per_flood) i = num_left;
            for(int a= 0; a< i; a++)
            {
                int x = Random.Range(0,mapGenerator.map_width - 1);
                int y = Random.Range(0,mapGenerator.map_height - 1);
                if (Obs_map[x,y]!= -2 && Obs_map[x,y]!= -1)
                {
                    cur_obs_pos.Add(Tuple.Create(x, y));
                    Obs_map[x, y] = -1;
                }
                else
                {
                    a--;
                }
            }
            if (flood())
            {
                for(int a = 0; a<i; a++)
                {
                    Obs_pos.Add(cur_obs_pos[a]);
                }
                num_left -= i;
            }
            else { try_num++; }

            for(int a= 0; a<mapGenerator.map_width; a++)
            {
                for(int b = 0; b< mapGenerator.map_height; b++)
                {
                    Obs_map[a,b] = 0;
                }
            }

        }

        foreach(Tuple<int,int> t in Obs_pos)
        {
            if (levelmap[t.Item1,t.Item2] == 0)
            {
                Instantiate(Obs_prefab_land, getLevelmapPositionInWorldSpace(t), Quaternion.identity, levelGameObject.transform);
            }
            else
            {
                Instantiate(Obs_prefab_water, getLevelmapPositionInWorldSpace(t), Quaternion.identity, levelGameObject.transform);
            }
        }
    }

    private bool flood()
    {
        int count = 0;
        dfs(node_pos[0].Item1, node_pos[0].Item2,ref count);
        Debug.Log(count);
        if(count == 5) { 
            return true;
        }
        else
        {
            return false;
        }
    }

    public void dfs(int i, int j, ref int count)
    {
        if (Obs_map[i, j] == -2)
        {
            count++;
        }
        Obs_map[i, j] = 1;
        if (i - 1 >= 0 && Obs_map[i-1,j] != -1 && Obs_map[i - 1, j] != 1) dfs(i - 1, j, ref count);

        if (i + 1 < mapGenerator.map_width && Obs_map[i + 1, j] != -1 && Obs_map[i + 1, j] != 1) dfs(i + 1, j, ref count);

        if (j - 1 >= 0 && Obs_map[i, j - 1] != -1 && Obs_map[i, j - 1] != 1) dfs(i, j - 1, ref count);

        if (j + 1 < mapGenerator.map_height && Obs_map[i, j + 1] != -1 && Obs_map[i, j + 1] != 1) dfs(i, j + 1, ref count);
    }
}