using UnityEngine;
using static MapNodeController;

public class AttackerController : BaseUnitController
{
    public override void SelectGoal()
    {
        Debug.LogWarning("Attacker");

        MapNodeController target = m_gameManager.closestNode(transform.position, Owner, false);

        Debug.Log(target);
        selectedGoalNode = target.transform;
    }
}