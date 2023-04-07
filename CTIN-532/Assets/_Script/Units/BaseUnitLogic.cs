using System;
using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

public class BaseUnitLogic : MonoBehaviour
{
    public AudioClip FightSound;

    [SerializeField]
    public Player Owner { get; protected set; }

    [SerializeField]
    protected Vector2Int CurrentCoordinates;

    [SerializeField]
    protected Vector2Int TargetCoordinates;

    [SerializeField]
    protected Transform Target;

    [SerializeField]
    protected HitBox hitBox;

    [SerializeField]
    protected HurtBox hurtBox;

    [SerializeField]
    protected float SpeedPoints;

    [SerializeField]
    private float timeBetweenMovesInSeconds;

    private const float BaseTimeBetweenMovesInSeconds = 1.0f;

    [SerializeField]
    private float timeRemainingUntilNextMoveInSeconds;

    private float timeBetweenTargetSelections = 1.0f;

    [SerializeField]
    private float timeUntilNextTargetSelection;

    [SerializeField]
    protected float TimeUntilNextChaseTargetSelection;

    [SerializeField]
    protected Transform ChaseTarget;

    [SerializeField]
    protected float MaxChaseDistance = 10.0f;

    private GameManager gameManager;


    void Awake()
    {
        CurrentCoordinates = new Vector2Int();
        timeUntilNextTargetSelection = 0;
        TimeUntilNextChaseTargetSelection = 0;
        timeRemainingUntilNextMoveInSeconds = timeBetweenMovesInSeconds;
        hitBox = GetComponent<HitBox>();
        hurtBox = GetComponent<HurtBox>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public BaseUnitLogic Initialize(Player owner, int xCoordinate, int yCoordinate, float hitPoints, float damagePoints, float speedPoints)
    {
        Owner = owner;
        CurrentCoordinates = new Vector2Int(xCoordinate, yCoordinate);
        SetUnitStats(hitPoints, damagePoints, speedPoints);
        return this;
    }

    private void SetUnitStats(float hitPoints, float damagePoints, float speedPoints)
    {
        // TODO: validate inputs
        this.hitBox.SetHitPoints(hitPoints);
        this.hurtBox.Damage = damagePoints;
        this.SpeedPoints = speedPoints;

        if (speedPoints != 0)
        {
            timeBetweenMovesInSeconds = BaseTimeBetweenMovesInSeconds / (float)Math.Sqrt(SpeedPoints);
        }
        else
        {
            timeBetweenMovesInSeconds = BaseTimeBetweenMovesInSeconds;
        }
        timeRemainingUntilNextMoveInSeconds = timeBetweenMovesInSeconds;
    }
    void FixedUpdate()
    {
        if (Target == null)
        {
            if (timeUntilNextTargetSelection <= 0)
            {
                SelectTarget();
                timeUntilNextTargetSelection = timeBetweenTargetSelections;
            }
            else
            {
                timeUntilNextTargetSelection -= Time.deltaTime;
            }
            return;
        }

        // Check the nearest enemy periodically to see if a target should be chased,
        // deviating off path from the current target goal temporarily:
        TimeUntilNextChaseTargetSelection -= Time.deltaTime;
        if (TimeUntilNextChaseTargetSelection < 0)
        {
            SelectChaseTarget();
            TimeUntilNextChaseTargetSelection = 5 * timeBetweenTargetSelections;
        }
        if (ChaseTarget != null)
        {
            if (Vector3.Distance(ChaseTarget.position, transform.position) > MaxChaseDistance)
            {
                ChaseTarget = null;
            }

        }

        // If unit reached target, select next map tile
        if (Math.Round((decimal)transform.position.x) == Math.Round((decimal)CurrentCoordinates.x)
            && Math.Round((decimal)transform.position.z) == Math.Round((decimal)CurrentCoordinates.y))
        {
            SelectNextMapTile();
        }
        else if (Math.Round((decimal)transform.position.x) == Math.Round((decimal)Target.position.x)
            && Math.Round((decimal)transform.position.z) == Math.Round((decimal)Target.position.y))
        {
            SelectTarget();
        }
        else
        {
            // If the nearest enemy is within the chase distance, chase it before continuing on the path to the target:
            if (ChaseTarget != null)
            {
                transform.LookAt(new Vector3(ChaseTarget.transform.position.x, transform.position.y, ChaseTarget.transform.position.z));
            }
            else
            {
                transform.LookAt(new Vector3(CurrentCoordinates.x, transform.position.y, CurrentCoordinates.y));
            }

            float magnitude = Time.fixedDeltaTime / timeBetweenMovesInSeconds;
            transform.position += transform.forward * magnitude;
        }
    }

    protected virtual void SelectChaseTarget()
    {
        var targetPlayer = Player.AI;
        if (Owner == Player.AI)
        {
            targetPlayer = Player.Human;
        }
        var nearestEnemy = gameManager.GetClosestUnitByPlayer(transform.position, targetPlayer);
        if (nearestEnemy != null)
        {
            ChaseTarget = nearestEnemy.transform;
        }
    }

    protected virtual void SelectTarget()
    {
        var gameManager = FindObjectOfType<GameManager>();

        if (this.Owner == Player.Human)
        {
            Target = gameManager.GetRandomNodeByPlayerOrNeutral(Player.AI).transform;
        }
        else
        {
            Target = gameManager.GetRandomNodeByPlayerOrNeutral(Player.Human).transform;
        }
        TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
    }

    protected void SelectNextMapTile()
    {
        List<Vector2Int> frontier = new();
        frontier.Add(new Vector2Int(CurrentCoordinates.x + 1, CurrentCoordinates.y));
        frontier.Add(new Vector2Int(CurrentCoordinates.x - 1, CurrentCoordinates.y));
        frontier.Add(new Vector2Int(CurrentCoordinates.x, CurrentCoordinates.y + 1));
        frontier.Add(new Vector2Int(CurrentCoordinates.x, CurrentCoordinates.y - 1));

        // TODO: Check the tile is passable.

        Vector2Int nextMapPosition = new();
        float nextDistance = float.MaxValue;
        foreach (var position in frontier)
        {
            float tempDistance = Vector2.Distance(position, new Vector2Int((int)Target.position.x, (int)Target.position.z));
            if (tempDistance < nextDistance || (tempDistance == nextDistance && UnityEngine.Random.Range(0, 1.0f) > 0.5f)) // Flip a coin for a tie.
            {
                nextMapPosition = position;
                nextDistance = tempDistance;
            }
        }

        // TODO: Handle the map scaling (this currently works because it is set to 1).
        // TODO: Use update to interpolate this movement 
        CurrentCoordinates = nextMapPosition;
        //this.transform.position = new Vector3(nextMapPosition.x, this.transform.position.y, nextMapPosition.y);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        //HandleCollisionWithUnit(other);
        HandleCollisionWithGoal(other);
    }

    protected virtual void HandleCollisionWithGoal(Collider possibleGoal)
    {
        // On collision with its goal, set the goal to null (so that it will find a new goal during next update):
        if (possibleGoal != null && Target != null)
        {
            if (possibleGoal != null && (int)possibleGoal.transform.position.x == TargetCoordinates.x && (int)possibleGoal.transform.position.z == TargetCoordinates.y)
            {
                Target = null;
            }
        }
    }
}
