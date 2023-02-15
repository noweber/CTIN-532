using UnityEngine;
using static MapNodeController;

public class WizardController : BaseUnitController
{
    public override void SelectGoal()
    {
        BaseUnitController targetUnit = m_gameManager.closestEnermy(transform.position, Owner, true);

        if (targetUnit == null)
        {
            selectedGoalNode = m_gameManager.closestNode(transform.position, Owner, false).transform;
        }
        else
        {
            selectedGoalNode = targetUnit.preGoal;
        }
    }

}
