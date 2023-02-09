using Unity.VisualScripting;
using UnityEngine;
using static MapNodeController;

public class BaseUnitController : MonoBehaviour
{
    public AudioClip FightSound;

    
    public float hitPoints { get; private set; }

    [Min(1.0f)]
    public float MinSpeed = 2.0f;

    [Min(1.0f)]
    public float MaxSpeed = 4.0f;

    private float Speed;

    [Range(0, 100)]
    public float PercentChanceToWinFight = 50;

    public Player Owner;

    public Transform selectedGoalNode;

    protected GameManager m_gameManager;

    protected virtual void Awake()
    {
        this.Speed = Random.Range(Mathf.Min(MinSpeed, MaxSpeed), Mathf.Max(MinSpeed, MaxSpeed));
        m_gameManager = FindObjectOfType<GameManager>();
        if (Owner == Player.Human)
        {
            m_gameManager.Player_Units.Add(this);
        }
        else
        {
            m_gameManager.Enemy_Units.Add(this);
        }
    }

    protected virtual void Start()
    {
        SelectStarterGoal();
    }

    protected void OnDestroy()
    {
        if (Owner == Player.Human)
        {
            m_gameManager.Player_Units.Remove(this);
        }
        else
        {
            m_gameManager.Enemy_Units.Remove(this);
        }
    }

    public void FixedUpdate()
    {
        if (selectedGoalNode == null)
        {
            SelectGoal();
        }
        Move(Time.fixedDeltaTime);
    }

    protected virtual void Move(float deltaTime)
    {
        // Move towards the goal:
        Quaternion rotationBeforeLookat = transform.rotation;
        transform.LookAt(selectedGoalNode);
        Vector3 forward = transform.forward;
        forward.y = 0;
        transform.position += this.Speed * deltaTime * forward; // The unit should only move along the xz-plane.
        transform.rotation = rotationBeforeLookat;
    }

    public virtual void SelectStarterGoal()
    {
        if (Owner == Player.Human)
        {
            var randomSelectedNode = m_gameManager.getRandomSelectedNode();
            if (randomSelectedNode == null)
            {
                SelectRandomNodeOwnedByOpponent();
            }
            else
            {
                selectedGoalNode = randomSelectedNode.transform;
            }
        }
        else
        {
            SelectRandomNodeOwnedByOpponent();
        }
    }

    public virtual void SelectGoal()
    {
        SelectRandomNodeOwnedByOpponent();
    }

    protected void SelectRandomNodeOwnedByOpponent()
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
                selectedGoalNode = nodeController.transform;
                break;
            }
        }

        if (selectedGoalNode == null)
        {
            Debug.LogError("Failed to select a goal node");
        }
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        HandleCollisionWithUnit(other);
        HandleCollisionWithGoal(other);
    }

    protected virtual void HandleCollisionWithGoal(Collider possibleGoal)
    {
        // On collision with its goal, set the goal to null (so that it will find a new goal during next update):
        if (possibleGoal != null)
        {
            if (possibleGoal != null && possibleGoal.transform == selectedGoalNode)
            {
                selectedGoalNode = null;
            }
        }
    }

    protected virtual void HandleCollisionWithUnit(Collider possibleUnit)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        BaseUnitController unitController = possibleUnit.GetComponent<BaseUnitController>();
        if (unitController != null)
        {
            if (Owner == Player.Human && unitController.Owner != Owner)
            {
                if (Random.Range(0.0f, 1.0f) > (PercentChanceToWinFight / 100))
                {
                    AudioManager.Instance.PlaySFX(FightSound, 0.5f);
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(possibleUnit.transform.gameObject);
                }
            }
        }
    }
}
