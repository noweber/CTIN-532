using UnityEngine;

public class GoalSpawnController : MonoBehaviour
{
    [Header("The Object to Spawn")]
    [Tooltip("The game object (prefabs) to spawn when the mouse is clicked.")]
    public GameObject SpawnPrefab;

    public PlayerMoneyController MoneyController;

    private Vector3 mouseScreenPosition;

    private Vector3 mouseWorldPosition;

    // Update is called once per frame
    void Update()
    {
        if (this.MoneyController == null)
        {
            Debug.LogError("No reference to the player's money controller.");
            return;
        }

        // Update the mouse position:
        this.mouseScreenPosition = Input.mousePosition;

        Ray mouseToWorldRay = Camera.main.ScreenPointToRay(this.mouseScreenPosition);
        if (Physics.Raycast(mouseToWorldRay, out RaycastHit hit))
        {
            this.mouseWorldPosition = hit.point;
        }

        // Check for mouse left-click:
        if (Input.GetMouseButtonDown(0) && this.MoneyController.TotalMoney != 0)
        {
            if (SpawnPrefab != null)
            {
                GameObject spawnedObject = Instantiate(this.SpawnPrefab);
                spawnedObject.transform.position = this.mouseWorldPosition;
                GoalController goalController = spawnedObject.GetComponent<GoalController>();
                if(goalController != null)
                {
                    goalController.SetGoalAmount(this.MoneyController.TotalMoney);
                    goalController.SetGoalText("$" + this.MoneyController.TotalMoney);
                }
                this.MoneyController.SpendMoney(this.MoneyController.TotalMoney);
            }
        }
    }
}
