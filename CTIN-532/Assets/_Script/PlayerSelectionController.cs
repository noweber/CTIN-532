using UnityEngine;
using static MapNodeController;

public class PlayerSelectionController : MonoBehaviour
{
    public GameObject[] list_Of_UnitPrefab;

    public GameObject SelectedUnitPrefab;

    public Sprite SelectedUnitSprite;

    public MapNodeController SelectedMapNode;

    public Player Owner;

    public float SecondsBetweenSpawns = 0.5f;

    private float secondsSinceLastSpawn;

    public float BurstSpawnDuration = 1.0f;

    [Min(1)]
    public int BurstSpawnRate = 4;

    private float secondsLeftInSpawningBurst;

    private int type;

    // Start is called before the first frame update
    void Start()
    {
        secondsSinceLastSpawn = 0;
        secondsLeftInSpawningBurst = 0;
    }

    public void SelectUnit(GameObject prefab, Sprite sprite,int type)
    {
        SelectedUnitPrefab = prefab;
        SelectedUnitSprite = sprite;
        this.type = type;
        secondsLeftInSpawningBurst = BurstSpawnDuration;
    }

    // Update is called once per frame
    void Update()
    {
        secondsSinceLastSpawn += Time.deltaTime;
        while (secondsSinceLastSpawn >= SecondsBetweenSpawns)
        {
            secondsSinceLastSpawn -= SecondsBetweenSpawns;
            if (Owner == Player.Human)
            {
                if (SelectedUnitPrefab != null && SelectedMapNode != null && SelectedUnitSprite != null)
                {
                    if (SelectedMapNode.Owner == Owner)
                    {
                        int spawnCount = 1;
                        if (secondsLeftInSpawningBurst > 0)
                        {
                            spawnCount = BurstSpawnRate;
                        }
                        for (int i = 0; i < spawnCount; i++)
                        {
                            SelectedMapNode.SpawnUnit(list_Of_UnitPrefab[type], SelectedUnitSprite, transform);
                        }
                    }
                }
            }
            else if (Owner == Player.AI)
            {
                if (SelectedUnitPrefab != null && SelectedUnitSprite != null)
                {

                    // Select a random node owned by the AI player:
                    MapNodeController[] mapNodeControllers = Object.FindObjectsOfType<MapNodeController>();
                    System.Random randomNumberGenerator = new();
                    int countAwaitingShuffle = mapNodeControllers.Length;
                    while (countAwaitingShuffle > 1)
                    {
                        countAwaitingShuffle--;
                        int nextIndex = randomNumberGenerator.Next(countAwaitingShuffle + 1);
                        MapNodeController node = mapNodeControllers[nextIndex];
                        mapNodeControllers[nextIndex] = mapNodeControllers[countAwaitingShuffle];
                        mapNodeControllers[countAwaitingShuffle] = node;
                    }
                    foreach (MapNodeController nodeController in mapNodeControllers)
                    {
                        if (nodeController.Owner == Owner)
                        {
                            nodeController.SpawnUnit(SelectedUnitPrefab, SelectedUnitSprite, transform);
                            break;
                        }
                    }

                }
            }
        }

        if (secondsLeftInSpawningBurst > 0)
        {
            secondsLeftInSpawningBurst -= Time.deltaTime;
        }
    }
}
