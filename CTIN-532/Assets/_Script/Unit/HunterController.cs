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

    public override void OnTriggerEnter(Collider other)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        BaseUnitController unitController = other.GetComponent<BaseUnitController>();
        if (unitController != null)
        {
            if (unitController.Owner != Owner)
            {
                // TODO: Play a sound.
                // TODO: Create particle effects.
                // Debug.Log("Human to AI unit collision detected!");
                if (Random.Range(0.0f, 1.0f) > 0.25f)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(other.transform.gameObject);
                }
            }
        }
    }
}
