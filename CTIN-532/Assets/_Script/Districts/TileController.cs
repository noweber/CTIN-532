using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script.Districts
{
    // TODO: Ground/Water should always spawn
    // TODO: if obstacle exists, spawn that, too.
    public class TileController : MonoBehaviour
    {
        public bool IsPassable { get; private set; }

        public bool IsWater { get; private set; }

        public float TileSize { get; private set; } = 1; // TODO: Allow this to be set and adjust the size of the prefabs accordingly.

        public Vector2 TileCenterPosition { get; private set; }

        [SerializeField]
        private List<GameObject> GroundTilePrefabs;

        [SerializeField]
        private List<GameObject> WaterTilePrefabs;

        [SerializeField]
        private List<GameObject> ObstaclePrefabs;


        private GameObject activePrefab;

        private void Awake()
        {
            if (GroundTilePrefabs == null || GroundTilePrefabs.Count == 0)
            {
                Debug.LogError(nameof(GroundTilePrefabs));
            }
            if (WaterTilePrefabs == null || WaterTilePrefabs.Count == 0)
            {
                Debug.LogError(nameof(GroundTilePrefabs));
            }
            if (ObstaclePrefabs == null || ObstaclePrefabs.Count == 0)
            {
                Debug.LogError(nameof(GroundTilePrefabs));
            }
        }

        public TileController Instantiate(int x, int y, bool isWaterTile, bool hasObstacle)
        {
            TileCenterPosition = new Vector2(x + (TileSize / 2.0f), y + (TileSize / 2.0f));
            transform.position = new Vector3(x, transform.parent.position.y, y);
            SetPassableState(!hasObstacle);
            return this;
        }

        private void SetPassableState(bool isPassable)
        {
            IsPassable = isPassable;
            if (activePrefab != null)
            {
                Destroy(activePrefab);
            }
            if (IsPassable)
            {
                activePrefab = Instantiate(GroundTilePrefabs[Random.Range(0, GroundTilePrefabs.Count)], transform);
            }
            else
            {
                activePrefab = Instantiate(ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Count)], transform);
            }
        }
    }
}
