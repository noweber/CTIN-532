using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets._Script.Districts
{
    public class DistrictController : MonoBehaviour
    {
        [SerializeField]
        public int DistrictNumber { get; private set; }

        [SerializeField]
        private GameObject tilePrefab;

        // TODO: Store all of the tiles.

        // TODO: Store the map nodes

        private HashSet<UnitController> humanUnits;

        private HashSet<UnitController> aiUnits;

        private HashSet<MapNodeController> mapNodes;

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
        }

        private void Start()
        {
            CreateDistrict(DistrictNumber);
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

        private void ResetLevelData()
        {
            if (humanUnits != null)
            {
                foreach (var unit in humanUnits)
                {
                    Destroy(unit.gameObject);
                }
            }
            humanUnits = new();
            if (aiUnits != null)
            {
                foreach (var unit in aiUnits)
                {
                    Destroy(unit.gameObject);
                }
            }
            aiUnits = new();
            if (mapNodes != null)
            {
                foreach (var node in mapNodes)
                {
                    Destroy(node.gameObject);
                }
            }
            mapNodes = new();
        }

        public List<UnitController> GetUnitsByPlayer(bool getHumanUnits)
        {
            if (getHumanUnits)
            {
                return humanUnits.ToList();
            }
            return aiUnits.ToList();
        }

        public void AddUnit(UnitController unitToAdd, bool isHumanUnit)
        {
            if (isHumanUnit)
            {
                if (humanUnits.Contains(unitToAdd))
                {
                    Debug.LogError(nameof(unitToAdd));
                }
                else
                {
                    humanUnits.Add(unitToAdd);
                }
                return;
            }
            if (aiUnits.Contains(unitToAdd))
            {
                Debug.LogError(nameof(unitToAdd));
            }
            else
            {
                aiUnits.Add(unitToAdd);
            }
        }

        public void RemoveUnit(UnitController unitToRemove, bool isHumanUnit)
        {
            if (isHumanUnit)
            {
                if (!humanUnits.Contains(unitToRemove))
                {
                    Debug.LogError(nameof(unitToRemove));
                }
                else
                {
                    humanUnits.Remove(unitToRemove);
                }
                return;
            }
            if (!aiUnits.Contains(unitToRemove))
            {
                Debug.LogError(nameof(unitToRemove));
            }
            else
            {
                aiUnits.Remove(unitToRemove);
            }
        }

        public void CreateDistrict(int districtNumber)
        {
            ResetLevelData();
            var sizeOfDistrict = GetDistrictSize(districtNumber);
            CreateTiles(sizeOfDistrict);
            // TODO: place hqs
            // TODO: place additional nodes based on level size
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
            int size = 16 + Mathf.FloorToInt(Mathf.Pow((float)districtNumber, 0.8f));
            return new Vector2Int(size, size);
        }
    }
}
