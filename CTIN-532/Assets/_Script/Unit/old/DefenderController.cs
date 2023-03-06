using UnityEngine;
using static MapNodeController;

public class DefenderController : BaseUnitController
{
    public float radius = 8f;
    public BaseUnitController targetUnit;
    public override void SelectGoal()
    {
        // chase enermy within range around defending node
        BaseUnitController target = m_gameManager.closestEnermy(transform.position,Owner,false);

        if(Vector3.Distance(target.transform.position, PreGoal.position) > radius)
        {
            target = null;
        }

        if (target == null)
        {
            selectedGoalNode = PreGoal;
        }
        else
        {
            selectedGoalNode = target.transform;
        }

    }

    protected override void HandleCollisionWithGoal(Collider possibleGoal)
    {
        if (possibleGoal != null)
        {
            if (possibleGoal != null && possibleGoal.transform == selectedGoalNode)
            {
                selectedGoalNode = null;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (targetUnit != null)
            targetUnit.hunterTargeted = false;
    }
}
