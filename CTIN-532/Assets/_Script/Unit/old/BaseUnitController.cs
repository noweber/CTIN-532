using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;
using static PlayerSelection;

public class BaseUnitController : MonoBehaviour
{
    public AudioClip FightSound;

    [SerializeField]
    private float maxHp;
    private float currentHp;

    [SerializeField]
    private float attackPoints;

    [SerializeField]
    private float magicPoints;

    [SerializeField]
    private float armorPoints;

    [SerializeField]
    private float resistPoints;

    [SerializeField]
    private float speedPoints;

    public Player Owner;

    public Transform selectedGoalNode;

    protected GameManager m_gameManager;

    public Transform PreGoal;

    public bool hunterTargeted = false;

    private float secondsBeforeSelectingGoal = 0.5f;

    public BaseUnitController Initialize(Player owner, Transform defaultGoal)
    {
        Owner = owner;
        PreGoal = defaultGoal;
        return this;
    }

    public void SetUnitStats(float hitPoints, float attackPoints, float magicPoints, float armorPoints, float resistPoints, float speedPoints)
    {
        // TODO: validate inputs
        this.maxHp = hitPoints;
        this.currentHp = hitPoints;
        this.attackPoints = attackPoints;
        this.magicPoints = magicPoints;
        this.armorPoints = armorPoints;
        this.resistPoints = resistPoints;
        this.speedPoints = speedPoints;
    }


    protected virtual void Awake()
    {
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
        // SelectStarterGoal();
    }

    protected virtual void OnDestroy()
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
        if (secondsBeforeSelectingGoal > 0)
        {
            secondsBeforeSelectingGoal -= Time.fixedDeltaTime;
        }
        else
        {
            if (selectedGoalNode == null)
            {
                SelectGoal();
            }
            Move(Time.fixedDeltaTime);
        }
    }

    protected virtual void Move(float deltaTime)
    {
        // Move towards the goal:
        Quaternion rotationBeforeLookat = transform.rotation;
        transform.LookAt(selectedGoalNode);
        Vector3 forward = transform.forward;
        forward.y = 0;
        transform.position += this.speedPoints * deltaTime * forward;  // The unit should only move along the xz-plane.
        transform.rotation = rotationBeforeLookat;
    }

    /*public virtual void SelectStarterGoal()
    {
        SelectRandomNodeOwnedByOpponent();
    }*/

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
                PreGoal = selectedGoalNode;
                selectedGoalNode = null;
            }
        }
    }

    protected virtual void HandleCollisionWithUnit(Collider possibleUnit)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        BaseUnitController otherUnit = possibleUnit.GetComponent<BaseUnitController>();
        if (otherUnit != null)
        {
            if (Owner == Player.Human && Owner != otherUnit.Owner)
            {
                AudioManager.Instance.PlaySFX(FightSound, 0.5f);

                // Adjust HP:
                ReceiveDamage(this, otherUnit);
                ReceiveDamage(otherUnit, this);

                // Push Back:
                //PushUnitBack(this);
                //PushUnitBack(otherUnit);
            }
        }
    }

    private void ReceiveDamage(BaseUnitController damageReceiver, BaseUnitController damageDealer)
    {
        // If the damage dealer has at least 1 magic or attack point,
        // They deal that many points minus the receiver's defensive stats.
        // A minimum of 1 damage is always applied.
        if (damageDealer.magicPoints > 0)
        {
            damageReceiver.currentHp -= Mathf.Max(1, damageDealer.magicPoints - damageReceiver.resistPoints);
        }
        if (damageDealer.attackPoints > 0)
        {
            damageReceiver.currentHp -= Mathf.Max(1, damageDealer.attackPoints - damageReceiver.armorPoints);
        }

        if (damageReceiver.currentHp <= 0)
        {
            Destroy(damageReceiver.gameObject);
        }
        else
        {
            var hpBar = damageReceiver.GetComponentInChildren<Hpbar>();
            hpBar.updateHpBar(damageReceiver.maxHp, damageReceiver.currentHp);
        }
    }
}
