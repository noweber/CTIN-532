using UnityEngine;
using static MapNodeController;
using static PlayerSelection;

public class SelectedObjects : MonoBehaviour
{
    public GameObject[] list_Of_UnitPrefab;

    public GameObject SelectedUnitPrefab;


    // TODO: refactor these stats when SpawnUnit() is refactored.
    public float hitPoints;
    public float attackPoints;
    public float magicPoints;
    public float armorPoints;
    public float resistPoints;
    public float speedPoints;

    public MapNodeController SelectedMapNode { get; private set; }

    private MapNodeController firstNodeSelected;

    public Player Owner;

    public float SecondsBetweenSpawns = 0.5f;

    private float secondsSinceLastSpawn;

    public float BurstSpawnDuration = 1.0f;

    [Min(1)]
    public int BurstSpawnRate = 4;

    private float secondsLeftInSpawningBurst;

    private int type;

    public void SetSelectedMapNode(MapNodeController nodeController)
    {
        if (SelectedMapNode != null)
        {
            SelectedMapNode.Deselect();
        }
        if (firstNodeSelected == null)
        {
            firstNodeSelected = SelectedMapNode;
        }
        if (nodeController == null)
        {
            SelectedMapNode = firstNodeSelected;
        }
        else
        {
            SelectedMapNode = nodeController;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        secondsSinceLastSpawn = 0;
        secondsLeftInSpawningBurst = 0;
    }

    public void SelectUnitToSpawn(GameObject prefab, int type, float hp, float attack, float magic, float armor, float resist, float speed)
    {
        SelectedUnitPrefab = prefab;
        this.type = type;
        secondsLeftInSpawningBurst = BurstSpawnDuration;
        hitPoints = hp;
        attackPoints = attack;
        magicPoints = magic;
        armorPoints = armor;
        resistPoints = resist;
        speedPoints = speed;
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
                if (SelectedUnitPrefab != null && SelectedMapNode != null)
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
                            SpawnUnit(list_Of_UnitPrefab[type], SelectedMapNode.transform);
                        }
                    }
                }
            }
            else if (Owner == Player.AI)
            {
                // Select a random unit prefab:
                SelectedUnitPrefab = list_Of_UnitPrefab[Random.Range(0, list_Of_UnitPrefab.Length)];

                if (SelectedUnitPrefab != null)
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
                            SpawnUnit(SelectedUnitPrefab, nodeController.transform);
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

    private void SpawnUnit(GameObject unitPrefab, Transform parent)
    {
        var unit = Instantiate(unitPrefab, parent.position, Quaternion.identity, transform);
        // TODO: handle map scale factor on the unit's starting postion
        unit.GetComponent<MapUnitController>().Initialize(Owner, (int)parent.position.x, (int)parent.position.z, hitPoints, attackPoints, magicPoints, armorPoints, resistPoints, speedPoints);

        // TODO: Refactor this so that the prefab contains the stat data and the UI card reads that instead of the UI card passing it to the prefab.
        // BaseUnitController controller;// = unit.GetComponent<BaseUnitController>();

        // TODO: DRY
        // TODO: Refactor and remove this from here
        /*
        if (Owner == Player.Human)
        {
            BaseUnitController controller;
            switch (PlayerSelection.Instance.SelectedLogic)
            {
                case UnitLogic.Attack:
                    controller = unit.AddComponent<AttackerController>().Initialize(Player.Human, parent);
                    break;
                case UnitLogic.Defend:
                    controller = unit.AddComponent<DefenderController>().Initialize(Player.Human, parent);
                    unit.AddComponent<RandomMeander>();
                    break;
                case UnitLogic.Hunt:
                    controller = unit.AddComponent<HunterController>().Initialize(Player.Human, parent);
                    break;
                case UnitLogic.Intercept:
                    controller = unit.AddComponent<WizardController>().Initialize(Player.Human, parent);
                    break;
                case UnitLogic.Random:
                default:
                    controller = unit.AddComponent<BaseUnitController>().Initialize(Player.Human, parent);
                    break;
            }
            controller.FightSound = AudioManager.Instance.FightSound.clip;
            controller.SetUnitStats(hitPoints, attackPoints, magicPoints, armorPoints, resistPoints, speedPoints);
            controller.PreGoal = parent;
        }
        else
        {
            BaseUnitController controller = unit.GetComponent<BaseUnitController>();
            controller.SetUnitStats(hitPoints, attackPoints, magicPoints, armorPoints, resistPoints, speedPoints);
            controller.PreGoal = parent;
        }*/
    }
}
