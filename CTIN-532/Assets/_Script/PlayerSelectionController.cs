using UnityEngine;
using UnityEngine.UI;
using static MapNodeController;

public class PlayerSelectionController : MonoBehaviour
{
    public GameObject SelectedUnitPrefab;

    public Sprite SelectedUnitSprite;

    public MapNodeController SelectedMapNode;

    public Player Owner;

    public float SecondsBetweenSpawns;

    private float secondsSinceLastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        SecondsBetweenSpawns = 0.5f;
        secondsSinceLastSpawn = 0;
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
                        SelectedMapNode.SpawnUnit(SelectedUnitPrefab, SelectedUnitSprite, transform);
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
    }
}
