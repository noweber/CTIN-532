using UnityEngine;

public class UnitController : MonoBehaviour
{
    public float Speed;

    public float Awareness;

    private float secondsSinceLastGoalCheck;

    private GoalController selectedGoal;

    private void Awake()
    {
        this.Speed = Random.Range(1.0f, 2.0f);
        this.Awareness = Random.Range(1.0f, 8.0f);
        this.secondsSinceLastGoalCheck = 0;
    }

    private void Update()
    {
        this.secondsSinceLastGoalCheck += Time.deltaTime;

        if(this.secondsSinceLastGoalCheck > this.Awareness)
        {
            this.secondsSinceLastGoalCheck -= this.Awareness;
            this.SelectGoal();
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
            this.selectedGoal = bestGoal;
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
}
