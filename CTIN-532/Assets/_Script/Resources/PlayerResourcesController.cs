using System;
using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

public class PlayerResourcesController : MonoBehaviour
{
    public int NumberOfUnitsSupportedPerNodeControlled = 5;

    public Player PlayerToControlResourceFor;

    public ResourceCountUiController UnitResourceCountController;

    public ResourceCountUiController NodeResourceCountController;

    [SerializeField]
    private List<BaseUnitLogic> units;

    [SerializeField]
    private HashSet<MapNodeController> nodes;

    GameManager gameManager;

    public void AddUnit(BaseUnitLogic unit)
    {
        units.Add(unit);
        UpdateUnitCount();
    }

    public void RemoveUnit(BaseUnitLogic unit)
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
        return units.Count < nodes.Count * NumberOfUnitsSupportedPerNodeControlled;
    }
    public void AddNode(MapNodeController node)
    {
        if (!nodes.Contains(node))
        {
            nodes.Add(node);
            UpdateNodeCount();
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
        units = new List<BaseUnitLogic>();
        nodes = new HashSet<MapNodeController>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        // TODO: Find any starting units

        // Find any starting nodes:
        var nodeControllers = FindObjectsOfType<MapNodeController>();
        Debug.Log("Nodes found: " + nodeControllers.Length);
        foreach (var node in nodeControllers)
        {
            if (node.Owner == PlayerToControlResourceFor)
            {
                AddNode(node);
            }
        }
    }

    private void RemoveNullUnits()
    {
        List<BaseUnitLogic> nonNullUnits = new();
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
        if (UnitResourceCountController != null)
        {
            UnitResourceCountController.SetResourceCount(units.Count);
        }
    }
    private void UpdateNodeCount()
    {
        if (NodeResourceCountController != null)
        {
            NodeResourceCountController.SetResourceCount(nodes.Count);
        }
    }

    private void Update()
    {
        if (gameManager.resource_reset)
        {
            units.Clear();
            nodes.Clear();
            UpdateUnitCount();
            UpdateNodeCount();
            gameManager.resource_reset = false;
        }
    }
}
