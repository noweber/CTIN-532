using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static MapNodeController;

public class PlayerResourcesController : MonoBehaviour
{
    public int NumberOfUnitsSupportedPerNodeControlled = 5;

    public Player PlayerToControlResourceFor;

    public ResourceCountUiController CurrentNumberOfUnits;

    public ResourceCountUiController MaxNumberOfUnits;

    public ResourceCountUiController NodeResourceCountController;

    public ResourceCountUiController MaxNodesCount;

    [SerializeField]
    private List<UnitController> units;

    [SerializeField]
    public HashSet<MapNodeController> nodes;

    GameManager gameManager;

    public TextMeshProUGUI CountText;

    public void AddUnit(UnitController unit)
    {
        units.Add(unit);
        UpdateUnitCount();
    }

    public void RemoveUnit(UnitController unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
            UpdateUnitCount();
        }
    }

    public bool CanSupportAnAdditionalUnit()
    {
        RemoveNullUnits();
        if (PlayerToControlResourceFor == Player.Human)
        {
            return units.Count < nodes.Count * NumberOfUnitsSupportedPerNodeControlled;
        }
        else
        {
            return units.Count < GetMaxNumberOfUnitsThePlayerCanHave(); ;
        }
    }

    public int GetMaxNumberOfUnitsThePlayerCanHave()
    {
        if (nodes == null)
        {
            return 0;
        }
        return nodes.Count * (NumberOfUnitsSupportedPerNodeControlled + gameManager.DistrictNumber);
    }

    public void AddNode(MapNodeController node)
    {
        if (!nodes.Contains(node))
        {
            nodes.Add(node);
            UpdateNodeCount();
            MaxNumberOfUnits.SetResourceCount(GetMaxNumberOfUnitsThePlayerCanHave());
        }
    }

    public void RemoveNode(MapNodeController node)
    {
        if (nodes.Contains(node))
        {
            nodes.Remove(node);
            UpdateNodeCount();
        }
    }

    private void Awake()
    {
        units = new List<UnitController>();
        nodes = new HashSet<MapNodeController>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        // TODO: Find any starting units

        // Find any starting nodes:
        var nodeControllers = FindObjectsOfType<MapNodeController>();
        //Debug.Log("Nodes found: " + nodeControllers.Length);
        foreach (var node in nodeControllers)
        {
            if (node.Owner == PlayerToControlResourceFor)
            {
                AddNode(node);
            }
        }
        gameManager.FindMapNodes();
        MaxNodesCount.SetResourceCount(gameManager.MapNodes.Length);
    }

    private void RemoveNullUnits()
    {
        List<UnitController> nonNullUnits = new();
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] != null)
            {
                nonNullUnits.Add(units[i]);
            }
        }
        units = nonNullUnits;
    }

    private void UpdateUnitCount()
    {
        if (CurrentNumberOfUnits != null)
        {
            CurrentNumberOfUnits.SetResourceCount(units.Count);
        }
    }
    private void UpdateNodeCount()
    {
        if (NodeResourceCountController != null)
        {
            NodeResourceCountController.SetResourceCount(nodes.Count);
        }

        if (PlayerToControlResourceFor == Player.Human)
        {
            // TODO: temperate check for game end
            if (nodes.Count == 5 && gameManager.gameState >= 200)
            {
                gameManager.DistrictNumber++;
                gameManager.gameState++;
                gameManager.resetGame();
            }
            else if (nodes.Count == 5 && (gameManager.gameState > 0 && gameManager.gameState < 200))
            {
                gameManager.gameState = 100;
            }
        }
        else if (PlayerToControlResourceFor == Player.AI)
        {
            //Debug.Log("Node Count AI: "  + nodes.Count);
            if (nodes.Count == 5 && gameManager.gameState >= 200)
            {
                //Debug.Log("GameLose");
                gameManager.gameState = 300;

            }
            else if (nodes.Count == 5 && gameManager.gameState > 0 && gameManager.gameState < 200)
            {
                gameManager.gameState = 100;
            }
        }
    }

    private void Update()
    {
        if (gameManager.resource_reset && PlayerToControlResourceFor == Player.Human)
        {
            units.Clear();
            nodes.Clear();
            UpdateUnitCount();
            UpdateNodeCount();
            gameManager.resource_reset = false;
        }
        else if (gameManager.resource_reset_AI && PlayerToControlResourceFor == Player.AI)
        {
            units.Clear();
            nodes.Clear();
            UpdateUnitCount();
            UpdateNodeCount();
            gameManager.resource_reset_AI = false;
        }
    }
}
