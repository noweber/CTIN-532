using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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
    protected float MaxHp;
    protected float CurrentHp;

    [SerializeField]
    protected float AttackPoints;

    [SerializeField]
    protected float MagicPoints;

    [SerializeField]
    protected float ArmorPoints;

    [SerializeField]
    protected float ResistPoints;

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

    public BaseUnitLogic Initialize(Player owner, int xCoordinate, int yCoordinate, float hitPoints, float attackPoints, float magicPoints, float armorPoints, float resistPoints, float speedPoints)
    {
        Owner = owner;
        CurrentCoordinates = new Vector2Int(xCoordinate, yCoordinate);
        SetUnitStats(hitPoints, attackPoints, magicPoints, armorPoints, resistPoints, speedPoints);
        return this;
    }

    private void SetUnitStats(float hitPoints, float attackPoints, float magicPoints, float armorPoints, float resistPoints, float speedPoints)
    {
        // TODO: validate inputs
        this.MaxHp = hitPoints;
        this.CurrentHp = hitPoints;
        this.AttackPoints = attackPoints;
        this.MagicPoints = magicPoints;
        this.ArmorPoints = armorPoints;
        this.ResistPoints = resistPoints;
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

    private void Awake()
    {
        CurrentCoordinates = new Vector2Int();
        timeUntilNextTargetSelection = 0;
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

        // TODO: Move the unit towards its target

        // TODO: If unit reached target, select next map tile
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
            float magnitude = Time.fixedDeltaTime / timeBetweenMovesInSeconds;
            transform.LookAt(new Vector3(CurrentCoordinates.x, transform.position.y, CurrentCoordinates.y));
            transform.position += transform.forward * magnitude;
        }
    }

    protected virtual void SelectTarget()
    {
        var gameManager = FindObjectOfType<GameManager>();
        // TODO: Refactor this to use a reference to the level and not the game manager.
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
        HandleCollisionWithUnit(other);
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

    protected virtual void HandleCollisionWithUnit(Collider possibleUnit)
    {
        // If an AI unit collides wtih a human unit, randomly destroy one of them.
        BaseUnitLogic otherUnit = possibleUnit.GetComponent<BaseUnitLogic>();
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

    private void ReceiveDamage(BaseUnitLogic damageReceiver, BaseUnitLogic damageDealer)
    {
        // If the damage dealer has at least 1 magic or attack point,
        // They deal that many points minus the receiver's defensive stats.
        // A minimum of 1 damage is always applied.
        if (damageDealer.MagicPoints > 0)
        {
            damageReceiver.CurrentHp -= Mathf.Max(1, damageDealer.MagicPoints - damageReceiver.ResistPoints);
        }
        if (damageDealer.AttackPoints > 0)
        {
            damageReceiver.CurrentHp -= Mathf.Max(1, damageDealer.AttackPoints - damageReceiver.ArmorPoints);
        }

        if (damageReceiver.CurrentHp <= 0)
        {
            Destroy(damageReceiver.gameObject);
        }
        else
        {
            var hpBar = damageReceiver.GetComponentInChildren<Hpbar>();
            hpBar.updateHpBar(damageReceiver.MaxHp, damageReceiver.CurrentHp);
        }
    }
}
