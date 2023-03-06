using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

public class MapUnitController : MonoBehaviour
{
    public AudioClip FightSound;

    [SerializeField]
    private Player Owner;

    [SerializeField]
    private Vector2Int currentCoordinates;

    [SerializeField]
    private Vector2Int targetCoordinates;

    [SerializeField]
    private Transform target;

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

    [SerializeField]
    private float timeBetweenMovesInSeconds;

    private const float BaseTimeBetweenMovesInSeconds = 1.0f;

    private float timeRemainingUntilNextMoveInSeconds;

    public MapUnitController Initialize(Player owner, int xCoordinate, int yCoordinate, float hitPoints, float attackPoints, float magicPoints, float armorPoints, float resistPoints, float speedPoints)
    {
        Owner = owner;
        currentCoordinates = new Vector2Int(xCoordinate, yCoordinate);
        SetUnitStats(hitPoints, attackPoints, magicPoints, armorPoints, resistPoints, speedPoints);
        return this;
    }

    private void SetUnitStats(float hitPoints, float attackPoints, float magicPoints, float armorPoints, float resistPoints, float speedPoints)
    {
        // TODO: validate inputs
        this.maxHp = hitPoints;
        this.currentHp = hitPoints;
        this.attackPoints = attackPoints;
        this.magicPoints = magicPoints;
        this.armorPoints = armorPoints;
        this.resistPoints = resistPoints;
        this.speedPoints = speedPoints;
        
        if (speedPoints != 0)
        {
            timeBetweenMovesInSeconds = BaseTimeBetweenMovesInSeconds / speedPoints;
        }
        else
        {
            timeBetweenMovesInSeconds = BaseTimeBetweenMovesInSeconds;
        }
        timeRemainingUntilNextMoveInSeconds = timeBetweenMovesInSeconds;
    }

    private void Awake()
    {
        currentCoordinates = new Vector2Int();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        if (target == null)
        {
            SelectTarget();
            return;
        }

        timeRemainingUntilNextMoveInSeconds -= Time.deltaTime;
        if (timeRemainingUntilNextMoveInSeconds <= 0)
        {
            MoveToNextMapTile();
            timeRemainingUntilNextMoveInSeconds = timeBetweenMovesInSeconds;
        }
    }

    protected void SelectTarget()
    {
        var gameManager = FindObjectOfType<GameManager>();
        Debug.Log("Selecting target.");
        // TODO: Refactor this to use a reference to the level and not the game manager.
        if (this.Owner == Player.Human)
        {
            target = gameManager.GetRandomNodeByPlayerOrNeutral(Player.AI).transform;
        }
        else
        {
            target = gameManager.GetRandomNodeByPlayerOrNeutral(Player.Human).transform;
        }
        targetCoordinates = new Vector2Int((int)target.transform.position.x, (int)target.transform.position.z);
    }

    protected void MoveToNextMapTile()
    {
        List<Vector2Int> frontier = new();
        frontier.Add(new Vector2Int(currentCoordinates.x + 1, currentCoordinates.y));
        frontier.Add(new Vector2Int(currentCoordinates.x - 1, currentCoordinates.y));
        frontier.Add(new Vector2Int(currentCoordinates.x, currentCoordinates.y + 1));
        frontier.Add(new Vector2Int(currentCoordinates.x, currentCoordinates.y - 1));

        // TODO: Check the tile is passable.

        Vector2Int nextMapPosition = new();
        float nextDistance = float.MaxValue;
        foreach (var position in frontier)
        {
            float tempDistance = Vector2.Distance(position, new Vector2Int((int)target.position.x, (int)target.position.z));
            if (tempDistance < nextDistance || (tempDistance == nextDistance && Random.Range(0, 1.0f) > 0.5f)) // Flip a coin for a tie.
            {
                nextMapPosition = position;
                nextDistance = tempDistance;
            }
        }

        // TODO: Handle the map scaling (this currently works because it is set to 1).
        // TODO: Use update to interpolate this movement 
        currentCoordinates = nextMapPosition;
        this.transform.position = new Vector3(nextMapPosition.x, this.transform.position.y, nextMapPosition.y);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        HandleCollisionWithUnit(other);
        HandleCollisionWithGoal(other);
    }

    protected virtual void HandleCollisionWithGoal(Collider possibleGoal)
    {
        // On collision with its goal, set the goal to null (so that it will find a new goal during next update):
        if (possibleGoal != null && target != null)
        {
            if (possibleGoal != null && (int)possibleGoal.transform.position.x == targetCoordinates.x && (int)possibleGoal.transform.position.z == targetCoordinates.y)
            {
                target = null;
            }
        }
    }

    protected virtual void HandleCollisionWithUnit(Collider possibleUnit)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        MapUnitController otherUnit = possibleUnit.GetComponent<MapUnitController>();
        if (otherUnit != null)
        {
            if (Owner == Player.Human && Owner != otherUnit.Owner)
            {
                AudioManager.Instance.PlaySFX(FightSound, 0.5f);

                // Adjust HP:
                ReceiveDamage(this, otherUnit);
                ReceiveDamage(otherUnit, this);
            }
        }
    }

    private void ReceiveDamage(MapUnitController damageReceiver, MapUnitController damageDealer)
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
