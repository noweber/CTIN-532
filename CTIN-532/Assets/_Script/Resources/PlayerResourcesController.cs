using Assets._Script;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static MapNodeController;

public class PlayerResourcesController : MonoBehaviour
{
    [SerializeField]
    public int NumberOfUnitsSupportedPerNodeControlled { get; private set; } = 5;

    public Player PlayerToControlResourceFor;

    public ResourceCountUiController CurrentNumberOfUnits;

    public ResourceCountUiController MaxNumberOfUnits;

    public ResourceCountUiController NodeResourceCountController;

    public ResourceCountUiController MaxNodesCount;

    [SerializeField]
    private int unitCount;

    [SerializeField]
    private int nodeCount;

    public TextMeshProUGUI CountText;

    public void AddUnit(UnitController unit)
    {
        unitCount++;
        UpdateUnitCount();
    }

    public void RemoveUnit(UnitController unit)
    {
        if (unitCount == 0)
        {
            Debug.LogError(nameof(unitCount));
            return;
        }
        unitCount--;
        UpdateUnitCount();
    }

    public bool CanSupportAnAdditionalUnit()
    {
        if (PlayerToControlResourceFor == Player.Human)
        {
            return unitCount < nodeCount * NumberOfUnitsSupportedPerNodeControlled;
        }
        else
        {
            return unitCount < GetMaxNumberOfUnitsThePlayerCanHave(); ;
        }
    }

    public int GetMaxNumberOfUnitsThePlayerCanHave()
    {
        if (PlayerToControlResourceFor == Player.AI)
        {
            return nodeCount * (NumberOfUnitsSupportedPerNodeControlled + DependencyService.Instance.Game().DistrictNumber);
        }
        else
        {
            return nodeCount * (NumberOfUnitsSupportedPerNodeControlled);
        }
    }

    public void AddNode()
    {
        UpdateNodeCount();
        UpdateUnitCount();
    }

    public void RemoveNode()
    {
        UpdateNodeCount();
        UpdateUnitCount();
    }

    private void Awake()
    {
        unitCount = 0;
        nodeCount = 1;
    }

    private void UpdateUnitCount()
    {
        if (CurrentNumberOfUnits != null)
        {
            CurrentNumberOfUnits.SetResourceCount(unitCount);
        }
        if (MaxNumberOfUnits != null)
        {
            MaxNumberOfUnits.SetResourceCount(GetMaxNumberOfUnitsThePlayerCanHave());
        }
    }

    private void UpdateNodeCount()
    {
        var nodeControllers = FindObjectsOfType<MapNodeController>();
        if (nodeControllers != null)
        {
            if (MaxNodesCount != null)
            {
                MaxNodesCount.SetResourceCount(nodeControllers.Length);
            }
            int nodesOwned = 0;
            foreach (var node in nodeControllers)
            {
                if (node.Owner == PlayerToControlResourceFor)
                {
                    nodesOwned++;
                }
            }
            nodeCount = nodesOwned;
            if (NodeResourceCountController != null)
            {
                NodeResourceCountController.SetResourceCount(nodeCount);
            }
        }

        if (PlayerToControlResourceFor == Player.Human)
        {
            // TODO: temperate check for game end
            if (nodeCount == 5 && DependencyService.Instance.Game().gameState >= 200)
            {
                DependencyService.Instance.Game().DistrictNumber++;
                DependencyService.Instance.Game().gameState++;
                DependencyService.Instance.Game().resetGame();
            }
            else if (nodeCount == 5 && (DependencyService.Instance.Game().gameState > 0 && DependencyService.Instance.Game().gameState < 200))
            {
                DependencyService.Instance.Game().gameState = 100;
            }
        }
        else if (PlayerToControlResourceFor == Player.AI)
        {
            //Debug.Log("Node Count AI: "  + nodes.Count);
            if (nodeCount == 5 && DependencyService.Instance.Game().gameState >= 200)
            {
                //Debug.Log("GameLose");
                DependencyService.Instance.Game().gameState = 300;

            }
            else if (nodeCount == 5 && DependencyService.Instance.Game().gameState > 0 && DependencyService.Instance.Game().gameState < 200)
            {
                DependencyService.Instance.Game().gameState = 100;
            }
        }
    }

    private void LateUpdate()
    {
        if (DependencyService.Instance.Game().resource_reset && PlayerToControlResourceFor == Player.Human)
        {
            UpdateUnitCount();
            UpdateNodeCount();
            DependencyService.Instance.Game().resource_reset = false;
        }
        else if (DependencyService.Instance.Game().resource_reset_AI && PlayerToControlResourceFor == Player.AI)
        {
            UpdateUnitCount();
            UpdateNodeCount();
            DependencyService.Instance.Game().resource_reset_AI = false;
        }
    }
}
