using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    public TextMeshPro GoalText;

    public int GoalAmount { get; private set; }

    public AudioSource TriggerSound;

    public void Awake()
    {
        this.GoalAmount = 0;
    }

    public void SetGoalAmount(int goalAmount)
    {
        this.GoalAmount = goalAmount;
        //Debug.Log("Goal amount set: " + this.GoalAmount.ToString());
    }

    public void SetGoalText(string goalText)
    {
        if (this.GoalText != null && goalText != null)
        {
            this.GoalText.text = goalText;
        }
        else
        {
            Debug.Log("Failed to set goal text.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only allow human-controlled units to trigger goals.
        UnitController unitController = other.GetComponent<UnitController>();
        if(unitController != null)
        {
            if(!unitController.IsHumanUnit)
            {
                return;
            }
        }

        // Play the trigger sound and delte the game object:
        if (this.TriggerSound != null)
        {
            this.TriggerSound.Play();
        }
        Destroy(this.transform.gameObject, this.TriggerSound.clip.length);
    }
}
