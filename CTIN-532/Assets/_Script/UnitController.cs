using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Min(1.0f)]
    public float Speed;

    public float Awareness;

    public bool IsHumanUnit = true;

    private float secondsSinceLastGoalCheck;

    private Transform selectedGoal;

    private void Awake()
    {
        this.Speed = Random.Range(1.0f, Mathf.Max(8.0f, Speed));
        this.Awareness = Random.Range(1.0f, 8.0f);
        this.secondsSinceLastGoalCheck = 0;
    }

    private void Update()
    {
        if (IsHumanUnit)
        {
            this.secondsSinceLastGoalCheck += Time.deltaTime;

            if (this.secondsSinceLastGoalCheck > this.Awareness)
            {
                this.secondsSinceLastGoalCheck -= this.Awareness;
                this.SelectGoal();
            }
        } else if(this.selectedGoal == null)
        {
            // Find the human HQ and just move towards that.
            HeadquartersController[] hqControllers = Object.FindObjectsOfType<HeadquartersController>();
            foreach(HeadquartersController hq in hqControllers)
            {
                if(hq.IsControlledByHuman)
                {
                    this.selectedGoal = hq.transform;
                }
            }
        }

        if (this.selectedGoal == null)
        {
            return;
        }

        transform.LookAt(this.selectedGoal.transform);
        transform.position += this.Speed * Time.deltaTime * transform.forward;
    }

    private void SelectGoal()
    {
        GoalController[] goals = Object.FindObjectsOfType<GoalController>();
        if (goals != null && goals.Length > 0)
        {
            GoalController bestGoal = null;
            foreach (GoalController goalController in goals)
            {
                if (goalController != null)
                {
                    float goalScore = this.GetGoalScore(goalController);
                    if (bestGoal == null || goalScore > this.GetGoalScore(bestGoal))
                    {
                        bestGoal = goalController;
                    }
                }
                else
                {
                    Debug.LogError("Goal controller is null.");
                }
            }
            this.selectedGoal = bestGoal.transform;
            //Debug.Log("Goal Score: " + this.GetGoalScore(this.selectedGoal).ToString());
        }
    }

    private float GetGoalScore(GoalController goal)
    {
        if (goal == null || goal.transform == null || goal.transform.position == null)
        {
            Debug.LogError("Goal is null.");
            return 0;
        }
        float distance = Vector3.Distance(goal.transform.position, this.transform.position) + 1.0f;
        //Debug.Log("Goal Distance: " + distance.ToString());
        //Debug.Log("Goal Amount: " + goal.GoalAmount.ToString());
        return (float)goal.GoalAmount / distance;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        UnitController unitController = other.GetComponent<UnitController>();
        if (unitController != null)
        {
            if (unitController.IsHumanUnit && !this.IsHumanUnit)
            {
                Debug.Log("Human to AI unit collision detected!");
                if (Random.Range(0.0f, 1.0f) > 0.5f)
                {
                    Destroy(gameObject);
                } else
                {
                    Destroy(other.transform.gameObject);
                }
            }
        }
    }
}
