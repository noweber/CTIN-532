using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

namespace Assets._Script.Districts
{
    public class DistrictController : MonoBehaviour
    {
        public int DistrictNumber = 0;

        [SerializeField]
        public Vector2Int DistrictSize { get; private set; }

        [SerializeField]
        public int DefaultDistrictSize = 16;

        [SerializeField]
        private GameObject tilePrefab;

        private Dictionary<Vector2Int, TileController> mapTiles;

        private void Awake()
        {
            if (tilePrefab == null)
            {
                Debug.LogError(nameof(tilePrefab));
            }
            if (tilePrefab.GetComponent<TileController>() == null)
            {
                Debug.LogError("The tile prefab does not contain a tile controller.");
            }
            DistrictSize = new Vector2Int();
        }

        public void NextDistrict()
        {
            if (DistrictNumber < int.MaxValue)
            {
                DistrictNumber++;
            }
        }

        public void PreviousDistrict()
        {
            if (DistrictNumber > 0)
            {
                DistrictNumber /= 2;
            }
        }

        public bool IsTilePassable(int x, int y)
        {
            var tileKey = new Vector2Int(x, y);
            if (!mapTiles.ContainsKey(tileKey))
            {
                return false;
            }
            return mapTiles[tileKey].IsPassable;
        }

        public int NumberOfNodes()
        {
            return FindObjectsOfType<MapNodeController>().Length;
        }

        private void ResetDistrictData()
        {
            var units = GameObject.FindObjectsOfType<UnitController>();
            foreach (var unit in units)
            {
                Destroy(unit.gameObject);
            }
            var nodes = GameObject.FindObjectsOfType<MapNodeController>();
            foreach (var node in nodes)
            {
                Destroy(node.gameObject);
            }
            foreach (var controller in FindObjectsOfType<SelectedObjects>())
            {
                controller.ResetData();
            }
            foreach (var controller in FindObjectsOfType<PlayerResourcesController>())
            {
                controller.ResetData();
            }
        }

        public List<UnitController> GetUnitsByPlayer(bool getHumanUnits)
        {
            var units = GameObject.FindObjectsOfType<UnitController>();
            List<UnitController> result = new();
            foreach (var unit in units)
            {
                if ((getHumanUnits && unit.Owner == Player.Human) ||
                    (!getHumanUnits && unit.Owner == Player.AI)
                    )
                {
                    result.Add(unit);
                }
            }
            return result;
        }

        public List<MapNodeController> GetNodesByPlayer(bool getHumanNodes)
        {
            var nodes = GameObject.FindObjectsOfType<MapNodeController>();
            List<MapNodeController> result = new();
            foreach (var node in nodes)
            {
                if ((getHumanNodes && node.Owner == Player.Human) ||
                    (!getHumanNodes && node.Owner == Player.AI)
                    )
                {
                    result.Add(node);
                }
            }
            return result;
        }

        public void CreateDistrict(int? districtNumber = null)
        {
            if (districtNumber != null && districtNumber.HasValue)
            {
                DistrictNumber = districtNumber.Value;
            }
            ResetDistrictData();
            //// TODO: ResetLevelData();
            DistrictSize = GetDistrictSize(DistrictNumber);
            // TODO: CreateTiles(DistrictSize);
            // TODO: place hqs
            // TODO: place additional nodes based on level size

            // TEMP:
            var level = GameObject.FindObjectOfType<LevelMono>();
            level.CreateCaveMap(DistrictSize);
        }

        private void CreateTiles(Vector2Int mapSize)
        {
            if (mapTiles != null)
            {
                foreach (var tile in mapTiles)
                {
                    Destroy(tile.Value.gameObject);
                }
            }
            mapTiles = new();


            // TODO: make new water tiles
            // TODO: make new grass tiles
            // TODO: make new obstacle tiles
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    var currentTile = Instantiate(tilePrefab, transform).GetComponent<TileController>().Instantiate(i, j, false, false);
                    mapTiles.Add(new Vector2Int(i, j), currentTile);
                }
            }
        }

        private Vector2Int GetDistrictSize(int districtNumber)
        {
            int size = DefaultDistrictSize + 2 * districtNumber;// Mathf.FloorToInt(Mathf.Pow((float)districtNumber + 1, 0.75F));
            Debug.Log("District Number: " + districtNumber);
            Debug.Log("District Size: " + size);
            return new Vector2Int(size, size);
        }
    }
}