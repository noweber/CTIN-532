using UnityEngine;

public class HunterController : BaseUnitController
{
    public override void SelectGoal()
    {
        BaseUnitController target = m_gameManager.closestEnermy(transform.position);
        if (target == null)
        {
            selectedGoalNode = m_gameManager.closestNode(transform.position, Owner, false).transform;
        }
        selectedGoalNode = target.transform;
    }
}
