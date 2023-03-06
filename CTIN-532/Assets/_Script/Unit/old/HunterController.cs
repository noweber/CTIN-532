using UnityEngine;
using static MapNodeController;

public class HunterController : BaseUnitController
{
    public BaseUnitController targetUnit;
    public override void SelectGoal()
    {
        targetUnit = m_gameManager.closestEnermy(transform.position, Owner, false);

        if (targetUnit == null)
        {
            selectedGoalNode = m_gameManager.closestNode(transform.position, Owner, false).transform;
        }
        else
        {
            targetUnit.hunterTargeted = true;
            selectedGoalNode = targetUnit.transform;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(targetUnit!= null )
            targetUnit.hunterTargeted= false;
    }
}
