using Assets._Script;
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
    public int NodeCount { get; set; }

    public TextMeshProUGUI CountText;

    public void AddUnit()
    {
        unitCount++;
        UpdateUnitCount();
    }

    public void RemoveUnit()
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
            return unitCount < NodeCount * NumberOfUnitsSupportedPerNodeControlled;
        }
        else
        {
            return unitCount < GetMaxNumberOfUnitsThePlayerCanHave();
        }
    }

    public int GetMaxNumberOfUnitsThePlayerCanHave()
    {
        if (PlayerToControlResourceFor == Player.AI)
        {
            return NodeCount * (NumberOfUnitsSupportedPerNodeControlled + DependencyService.Instance.DistrictController().DistrictNumber);
        }
        else
        {
            return NodeCount * (NumberOfUnitsSupportedPerNodeControlled);
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
        ResetData();
    }

    public void ResetData()
    {
        unitCount = 0;
        NodeCount = 1;
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
            NodeCount = nodesOwned;
            if (NodeResourceCountController != null)
            {
                NodeResourceCountController.SetResourceCount(NodeCount);
            }
        }

        if (NodeCount > 0 && NodeCount == DependencyService.Instance.DistrictController().NumberOfNodes())
        {
            if (PlayerToControlResourceFor == Player.Human)
            {
                DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Victory);
            }
            else if (PlayerToControlResourceFor == Player.AI)
            {
                DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Defeat);
            }
        }
    }
}
