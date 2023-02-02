using UnityEngine;

public class HunterController : BaseUnitController
{
    public override void SelectGoal()
    {
        if (m_gameManager.Enemy_Units != null)
        {
            BaseUnitController target = m_gameManager.Enemy_Units[Random.Range(0, m_gameManager.Enemy_Units.Count)];
            if (target == null)
            {
                selectedGoalNode = m_gameManager.closestNode(transform.position, Owner, false).transform;
            }
            selectedGoalNode = target.transform;
        }
    }
}
