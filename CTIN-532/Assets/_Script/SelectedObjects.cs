using Assets._Script;
using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;
using static PlayerSelection;

public class SelectedObjects : MonoBehaviour
{
    public GameObject[] list_Of_UnitPrefab;

    public GameObject SelectedUnitPrefab;

    // TODO: refactor these stats when SpawnUnit() is refactored.
    public float hitPoints;
    public float damagePoints;
    public float speedPoints;

    public MapNodeController SelectedMapNode { get; private set; }

    private MapNodeController firstNodeSelected;

    public Player Owner;

    public float SecondsBetweenSpawns = 0.5f;

    private float secondsSinceLastSpawn;

    private int type;

    private PlayerSelection playerSelection;

    //private GameManager gameManager;

    public GameObject unitParent;

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
        //secondsSinceLastSpawn = 0;
        playerSelection = GetComponent<PlayerSelection>();
        //gameManager = FindObjectOfType<GameManager>();
        ResetData();
    }
    
    public void ResetData()
    {
        secondsSinceLastSpawn = 0;
        if (unitParent != null)
        {
            Destroy(unitParent);
        }
        unitParent = new GameObject("UnitParent");
    }

    public void SelectUnitToSpawn(GameObject prefab, int type, float hp, float damage, float speed)
    {
        SelectedUnitPrefab = prefab;
        this.type = type;
        hitPoints = hp;
        damagePoints = damage;
        speedPoints = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            return;
        }

        if (Owner == Player.Human)
        {
            return;
        }
        //if (!gameManager.spawn_enabled) { return; }

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
    }

    public void SpawnUnit(GameObject unitPrefab, Transform parent = null)
    {
        // TODO: Refactor this so that the game object does not exist outside of this state and does not need this dependency 
        if(DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            return;
        }

        PlayerResourcesController playerResources = PlayerResourcesManager.Instance.GetPlayerResourcesController(Owner);
        // This block of code checks whether or not the player can support an additional unit being spawned based on their resources.
        if (!playerResources.CanSupportAnAdditionalUnit())
        {
            return;
        }

        if (Owner == Player.Human)
        {
            if (!CurrencyController.Instance.CanAffordAnotherUnit())
            {
                return;
            }
            CurrencyController.Instance.PurchaseUnit();
        }

        if (parent == null)
        {
            parent = SelectedMapNode.transform;
        }

        if (unitParent == null) unitParent = new GameObject("UnitParent");
        var unit = Instantiate(unitPrefab, parent.position, Quaternion.identity, unitParent.transform);
        if (Owner == Player.Human)
        {
            unit.GetComponent<UnitController>().Initialize(Owner, (int)parent.position.x, (int)parent.position.z, hitPoints, damagePoints, speedPoints, playerSelection.SelectedLogic);
        }
        else
        {
            damagePoints = unitPrefab.GetComponent<HurtBox>().Damage;
            hitPoints = unitPrefab.GetComponent<HitBox>().MaxHitPoints;

            int randomLogic = Random.Range(0, 2);
            switch (randomLogic)
            {
                case 0:
                    unit.GetComponent<UnitController>().Initialize(Owner, (int)parent.position.x, (int)parent.position.z, hitPoints, damagePoints, speedPoints, UnitLogic.Attack);
                    break;
                default:
                    unit.GetComponent<UnitController>().Initialize(Owner, (int)parent.position.x, (int)parent.position.z, hitPoints, damagePoints, speedPoints, UnitLogic.Split);
                    break;
            }
        }

        // This adds the unit to the player's set of resources for tracking.
        playerResources.AddUnit();
    }
}
