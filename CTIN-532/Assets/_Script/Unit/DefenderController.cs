using UnityEngine;
using static MapNodeController;

public class DefenderController : BaseUnitController
{
    public override void SelectGoal()
    {
        MapNodeController target = m_gameManager.closestSelected(transform.position, Owner, true);
        if (target == null)
        {
            target = m_gameManager.closestNode(transform.position, Owner, true);
        }
        Debug.Log(target);
        selectedGoalNode = target.transform;
    }

    protected override void HandleCollisionWithGoal(Collider possibleGoal)
    {
        // On collision with its goal, do nothing.
    }
}
