using UnityEngine;
using static MapNodeController;

public class AttackerController : BaseUnitController
{
    public override void SelectGoal()
    {
        MapNodeController target = m_gameManager.closestSelected(transform.position,Owner,false);
        if(target == null)
        {
            target = m_gameManager.closestNode(transform.position, Owner, false);
        }
        Debug.Log(target);
        selectedGoalNode = target.transform;
        
    }
}