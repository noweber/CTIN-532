using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static MapNodeController;
using Random = UnityEngine.Random;
using System.Linq;
using Assets._Script;

/// <summary>
/// This class creates the game's entities procedurally which sets up a playable level within the game.
/// </summary>
public class LevelMono : MonoBehaviour
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
    private bool[,] obstacleBinaryMap;

    private SelectedObjects humanPlayerController;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsTilePassable(int x, int y)
    {
        return obstacleBinaryMap[x, y];
    }


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        obstacleBinaryMap = null;
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
    }

    public void CreateCaveMap(Vector2Int mapSize)
    {
        mapGenerator.CreateMap(mapSize);
        obstacleBinaryMap = mapGenerator.GetBinaryTilemap();
        //DebugUtils.PrintBoolArray(obstacleBinaryMap);
        //PlaceFortressNodes();
        int radius = (int)Math.Sqrt(((mapSize.x + mapSize.y) / 4) + 1);
        //int radius = DependencyService.Instance.DistrictController().DistrictNumber + 4;
        //Debug.Log("radius: " + radius);
        //var nodePositions = PoissonDiscSampling.GeneratePoints(obstacleBinaryMap, radius);
        var nodePositions = RandomSampling.GetRandomPointsFromGrid(obstacleBinaryMap, 1, 1 + (int)Math.Sqrt(DependencyService.Instance.DistrictController().DistrictNumber));
        if (nodePositions == null || !placePlayerHeadquarters())
        {
            Debug.LogError("Failed to generate node positions on the map.");
            CreateCaveMap(mapSize);
            return;
        }
        Debug.Log("Number of Nodes: " + (nodePositions.Count + 2));
        foreach (var position in nodePositions)
        {
            PlaceFortressNode(new Tuple<int, int>(position.x, position.y), false, false);
        }
    }

    /// <summary>
    /// Clears any current player headquarter's and places two new ones in two different random corners of the map.
    /// The HQs are always placed on passable tiles of the map (assuming these exist).
    /// Corners without passable tiles are searched for the nearest passable tile to place an HQ on.
    /// </summary>
    private bool placePlayerHeadquarters()
    {

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
        //.Log("Placing player one's HQ in corner: " + playerOneCornerIndex);
        //Debug.Log("Placing player two's HQ in corner: " + playerTwoCornerIndex);
        Tuple<int, int> playerOneCornerPosition = convertCornerIndexToXYTuple(playerOneCornerIndex);
        Tuple<int, int> playerTwoCornerPosition = convertCornerIndexToXYTuple(playerTwoCornerIndex);
        if (playerOneCornerPosition == null || playerTwoCornerPosition == null)
        {
            Debug.LogError("Failed to select corners for player positions.");
            return false;
        }

        // The corner tiles might not be passable, so a search must execute to find the nearest passable tile.
        Tuple<int, int> playerOneHqTilemapPosition = findNearestPassableTileBfs(playerOneCornerPosition);
        Tuple<int, int> playerTwoHqTilemapPosition = findNearestPassableTileBfs(playerTwoCornerPosition);
        if (playerOneHqTilemapPosition == null || playerTwoHqTilemapPosition == null)
        {
            Debug.LogError("Failed to select headquarter positions for the player positions.");
            return false;
        }

        if (HeadquartersPrefab != null)
        {
            // Place Player One's HQ:
            PlaceFortressNode(playerOneHqTilemapPosition, true);

            // Place Player Two's HQ:
            PlaceFortressNode(playerTwoHqTilemapPosition, false, true);
        }
        else
        {
            Debug.LogWarning("Headquarters prefab is null.");
        }
        CameraControl c = FindObjectOfType<CameraControl>();
        c.SetFocus(getLevelmapPositionInWorldSpace(playerOneHqTilemapPosition));
        return true;
    }

    private void PlaceFortressNode(Tuple<int, int> tilemapPosition, bool isHumanPlayerHq = false, bool isAiPlayerHq = false)
    {
        Vector3 fortressPosition = getLevelmapPositionInWorldSpace(tilemapPosition);
        GameObject fortressGameObject = Instantiate(HeadquartersPrefab, fortressPosition, Quaternion.identity, levelGameObject.transform);
        MapNodeController fortressMapNodeController = fortressGameObject.GetComponent<MapNodeController>();
        if (isHumanPlayerHq)
        {
            fortressMapNodeController.SetOwner(Player.Human);
            if (humanPlayerController != null)
            {
                humanPlayerController.SetSelectedMapNode(fortressMapNodeController);
            }
            else
            {
                Debug.LogError("The controller for the human player is null when trying to assign a newly placed HQ map node as the player's selection default.");
            }
        }
        else if (isAiPlayerHq)
        {
            fortressMapNodeController.SetOwner(Player.AI);
        }
        else
        {
            fortressMapNodeController.SetOwner(Player.Neutral);
        }

        fortressGameObject.transform.parent = levelGameObject.transform;
    }

    private Vector3 getLevelmapPositionInWorldSpace(Tuple<int, int> tilemapPosition)
    {
        if (obstacleBinaryMap == null)
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
        if (obstacleBinaryMap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return null;
        }

        return cornerIndex switch
        {
            0 => new Tuple<int, int>(0, 0),
            1 => new Tuple<int, int>(obstacleBinaryMap.GetLength(0) - 1, 0),
            2 => new Tuple<int, int>(obstacleBinaryMap.GetLength(0) - 1, obstacleBinaryMap.GetLength(1) - 1),
            _ => new Tuple<int, int>(0, obstacleBinaryMap.GetLength(1) - 1),
        };
    }

    /// <summary>
    /// Given an (x, y) position within the tilemap, this function uses breadth-first search to find the nearest passable tile that does not already contain another level entity.
    /// </summary>
    /// <param name="tilemapSearchStartPostiion">The given tilemap position (x, y) to begin the search from.</param>
    /// <returns>Returns a tilemap position (x,y) of the nearest passable tile to the search start position.</returns>
    private Tuple<int, int> findNearestPassableTileBfs(Tuple<int, int> tilemapSearchStartPostiion)
    {
        if (obstacleBinaryMap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return null;
        }

        // Reset the number of search positions that have been ignored to allow for some randomness:
        searchPositionsPassedOn = 0;

        // Create a queue to keep track of which tiles to search from next:
        Queue<Tuple<int, int>> searchFrontier = new();

        // Create a data structure to store which tiles of the map have already been visited:
        bool[,] cellsVisited = new bool[obstacleBinaryMap.GetLength(0), obstacleBinaryMap.GetLength(1)];

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
            if (IsTilePassable(x, y))
            {
                searchPositionsPassedOn++;
                if (searchPositionsPassedOn >= MinimumSearchPositionTolerance)
                {
                    if (searchPositionsPassedOn >= MaximumSearchPositionTolerance || Random.Range(0.0f, 1.0f) < SearchPositionToleranceChance)
                    {
                        //Debug.Log("Tiles skipped in search: " + searchPositionsPassedOn);
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

        if (cellsSearched == null)
        {
            Debug.LogError("Cells searched data is null.");
        }

        // To be valid, the cell to check must be within the map boundaries.
        if (x < 0 || x >= obstacleBinaryMap.GetLength(0) || y < 0 || y >= obstacleBinaryMap.GetLength(1))
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

    private void PlaceFortressNodes()
    {
        if (obstacleBinaryMap == null)
        {
            Debug.LogError("Tilemap data is null.");
            return;
        }

        //Debug.Log("Placing 5 nodes within the level.");

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
        int middlePosx = Random.Range((obstacleBinaryMap.GetLength(0) - 1) / 2 - middleOffset, (obstacleBinaryMap.GetLength(0) - 1) / 2 + middleOffset);
        int middlePosy = Random.Range((obstacleBinaryMap.GetLength(1) - 1) / 2 - middleOffset, (obstacleBinaryMap.GetLength(1) - 1) / 2 + middleOffset);
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
            PlaceFortressNode(nodeTilemapPosition_1, isHumanPlayerHq: true);
            PlaceFortressNode(nodeTilemapPosition_2);
            PlaceFortressNode(nodeTilemapPosition_3, isAiPlayerHq: true);
            PlaceFortressNode(nodeTilemapPosition_4);
            PlaceFortressNode(nodeTilemapPosition_5);
        }
        else
        {
            Debug.LogWarning("Headquarters prefab is null.");
        }

        CameraControl c = FindObjectOfType<CameraControl>();
        c.SetFocus(getLevelmapPositionInWorldSpace(nodeTilemapPosition_1));
    }

}