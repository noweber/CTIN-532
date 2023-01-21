using UnityEngine;

public class HeadquartersController : MonoBehaviour
{
    public GameObject AiSpawnerPrefab;

    public GameObject HumanSpawnerPrefab;

    public bool IsControlledByHuman = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!IsControlledByHuman && AiSpawnerPrefab != null)
        {
            Instantiate(AiSpawnerPrefab, transform);
        } else
        {
            Instantiate(HumanSpawnerPrefab, transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // If human unit collides with an AI HQ or vise versa, that team wins:
        UnitController unitController = other.GetComponent<UnitController>();
        if (unitController != null)
        {
            if (unitController.IsHumanUnit && !this.IsControlledByHuman)
            {
                Debug.Log("You win!");
                Destroy(gameObject);
            } else if(!unitController.IsHumanUnit && this.IsControlledByHuman)
            {
                Debug.Log("You lose!");
                Destroy(gameObject);
            }
        }
    }
}
