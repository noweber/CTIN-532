using UnityEngine;
using static MapNodeController;

public class UnitController : MonoBehaviour
{
    [Min(1.0f)]
    public float Speed;

    public Player Owner;

    private MapNodeController selectedGoalNode;

    private void Awake()
    {
        this.Speed = Random.Range(2.0f, 4.0f);
    }

    public void SetSprite(Sprite unitSprite)
    {
        GetComponent<SpriteRenderer>().sprite = unitSprite;
    }

    public void Update()
    {
        if (selectedGoalNode == null || selectedGoalNode.Owner == Owner)
        {
            SelectGoal();
            return;
        }

        // Move towards the goal:
        Quaternion rotationBeforeLookat = transform.rotation;
        transform.LookAt(selectedGoalNode.transform);
        Vector3 forward = transform.forward;
        forward.y = 0;
        transform.position += this.Speed * Time.deltaTime * forward; // The unit should only move along the xz-plane.
        transform.rotation = rotationBeforeLookat;
    }

    private void SelectGoal()
    {
        // Select a random node not owned by this unit's player:
        MapNodeController[] mapNodeControllers = Object.FindObjectsOfType<MapNodeController>();

        // Shuffle the nodes randomly
        System.Random randomNumberGenerator = new();
        int countAwaitingShuffle = mapNodeControllers.Length;
        while (countAwaitingShuffle > 1)
        {
            countAwaitingShuffle--;
            int nextIndex = randomNumberGenerator.Next(countAwaitingShuffle + 1);
            MapNodeController node = mapNodeControllers[nextIndex];
            mapNodeControllers[nextIndex] = mapNodeControllers[countAwaitingShuffle];
            mapNodeControllers[countAwaitingShuffle] = node;
        }

        // Select the first node owned by a different player:
        foreach (MapNodeController nodeController in mapNodeControllers)
        {
            if (nodeController.Owner != Owner)
            {
                selectedGoalNode = nodeController;
                break;
            }
        }

        if (selectedGoalNode == null)
        {
            Debug.LogError("Failed to select a goal node");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // On collision with a node, select the next node to move to by deleting the goal node:
        MapNodeController nodeController = other.GetComponent<MapNodeController>();
        if (nodeController != null)
        {
            Debug.Log("Node conversion collision detected!");
            if (nodeController.Owner != Owner)
            {
                selectedGoalNode = null;
                Debug.Log("The unit has cleared its goal.");
            }
        }

        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        UnitController unitController = other.GetComponent<UnitController>();
        if (unitController != null)
        {
            if (unitController.Owner != Owner)
            {
                // TODO: Play a sound.
                // TODO: Create particle effects.
                Debug.Log("Human to AI unit collision detected!");
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
