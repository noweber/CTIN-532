using Assets._Script;
using Assets._Script.SFX;
using Assets._Script.Units;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static MapNodeController;
using static PlayerSelection;

public class UnitController : MonoBehaviour
{
    public AudioClip FightSound;

    [SerializeField]
    private GameObject LogicChangeParticlesPrefab;

    [SerializeField]
    public Player Owner { get; protected set; }

    [SerializeField]
    protected Stack<Vector2Int> CurrentPath;

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

    private float timeBetweenTargetSelections = 1.0f;

    [SerializeField]
    private float timeUntilNextTargetSelection;

    [SerializeField]
    protected float TimeUntilNextChaseTargetSelection;

    [SerializeField]
    protected Transform ChaseTarget;

    [SerializeField]
    protected float MaxChaseDistance = 10.0f;

    private float defenseRadius = 8f;

    [SerializeField]
    public UnitLogic CurrentLogic { get; private set; }

    private Vector2Int GetMapCoordinatesOfVector3(Vector3 position)
    {
        return new Vector2Int((int)Math.Round((decimal)position.x), (int)Math.Round((decimal)position.z));
    }

    private Vector2Int GetVectorFromCoordinatesTuple(Tuple<int, int> tuple)
    {
        return new Vector2Int(tuple.Item1, tuple.Item2);
    }
    private Tuple<int, int> GetTupleFromVector3(Vector3 position)
    {
        return new Tuple<int, int>((int)Math.Round((decimal)position.x), (int)Math.Round((decimal)position.z));
    }

    void Awake()
    {
        timeUntilNextTargetSelection = 0;
        TimeUntilNextChaseTargetSelection = 0;
        hitBox = GetComponent<HitBox>();
        hurtBox = GetComponent<HurtBox>();
        CurrentPath = new Stack<Vector2Int>();
    }

    public UnitController Initialize(Player owner, int xCoordinate, int yCoordinate, float hitPoints, float damagePoints, float speedPoints, UnitLogic logic)
    {
        Owner = owner;
        CurrentPath.Push(new Vector2Int(xCoordinate, yCoordinate));
        SetUnitStats(hitPoints, damagePoints, speedPoints);
        ChangeLogic(logic);
        return this;
    }
    void OnDestroy()
    {
        PlayerResourcesManager.Instance.GetPlayerResourcesController(Owner).RemoveUnit();
    }

    public void ChangeLogic(UnitLogic logic)
    {
        CurrentLogic = logic;
        var gameObject = Instantiate(LogicChangeParticlesPrefab, transform.position, Quaternion.identity, transform);
        var text = gameObject.GetComponent<FloatingTextController>();
        text.SetText(CurrentLogic.ToString());
        SelectTarget();
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
    }

    void FixedUpdate()
    {
        if (DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            Destroy(gameObject);
        }

        IsFacingLeft();
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
        if (Math.Round((decimal)transform.position.x) == Math.Round((decimal)Target.position.x)
            && Math.Round((decimal)transform.position.z) == Math.Round((decimal)Target.position.y))
        {
            SelectTarget();
        }
        else if (CurrentPath.Count == 0)
        {
            SelectPathTiles();
        }
        else if (Math.Round((decimal)transform.position.x) == Math.Round((decimal)CurrentPath.Peek().x)
            && Math.Round((decimal)transform.position.z) == Math.Round((decimal)CurrentPath.Peek().y))
        {
            CurrentPath.Pop();
            //SelectNextPathTile();
        }
        else
        {
            // The unit does not move while it is engaged in combat or being hit:
            if (!hitBox.IsBeingHit)
            {
                // If the nearest enemy is within the chase distance, chase it before continuing on the path to the target:
                float magnitude = UnityEngine.Random.Range(0.8f * Time.fixedDeltaTime / timeBetweenMovesInSeconds, Time.fixedDeltaTime / timeBetweenMovesInSeconds);
                if (ChaseTarget != null)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(ChaseTarget.transform.position.x, transform.position.y, ChaseTarget.transform.position.z), magnitude);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(CurrentPath.Peek().x, transform.position.y, CurrentPath.Peek().y), magnitude);
                }
            }
        }
    }

    public bool IsInCombat()
    {
        return hitBox.IsBeingHit;
    }

    /// <summary>
    /// A method for determining which direction a unit is facing.
    /// If the unit is not facing left, then it is facing right.
    /// </summary>
    /// <returns>
    /// Returns a bool of whether or not the unit is facing left.
    /// </returns>
    public bool IsFacingLeft()
    {
        if (ChaseTarget != null)
        {
            return ChaseTarget.position.x < transform.position.x;
        }
        else if (Target != null)
        {
            return Target.position.x < transform.position.x;
        }
        return false;
    }

    protected virtual void SelectChaseTarget()
    {
        if (CurrentLogic == UnitLogic.Defend)
        {
            return;
        }
        var nearestEnemy = UnitHelpers.GetNearestHostileUnit(this);
        if (nearestEnemy != null)
        {
            ChaseTarget = nearestEnemy.transform;
        }
    }

    protected void SelectPathTiles()
    {
        var errorMessage = MethodBase.GetCurrentMethod().DeclaringType.Name + "::" + MethodBase.GetCurrentMethod();

        // Assumes Target is not null:

        if (Target == null)
        {
            Debug.LogError(errorMessage + ": Target");
            return;
        }
        var levelMono = FindObjectOfType<LevelMono>();
        if (levelMono == null)
        {
            Debug.LogError(errorMessage + ": LevelMono");
            return;
        }

        // Clear the current path:
        CurrentPath.Clear();
        var pathTileTuples = ShortestPathFinder.FindShortestPath(levelMono.ObstacleBinaryMap, GetTupleFromVector3(transform.position), GetTupleFromVector3(Target.position));
        if (pathTileTuples == null || pathTileTuples.Count == 0)
        {
            // Rest the target and have the unit run in a random direction in case it gets stuck:
            SelectTarget();
            var currentCoordinates = GetMapCoordinatesOfVector3(transform.position);
            List<Vector2Int> frontier = new();
            frontier.Add(new Vector2Int(currentCoordinates.x + 1, currentCoordinates.y));
            frontier.Add(new Vector2Int(currentCoordinates.x - 1, currentCoordinates.y));
            frontier.Add(new Vector2Int(currentCoordinates.x, currentCoordinates.y + 1));
            frontier.Add(new Vector2Int(currentCoordinates.x, currentCoordinates.y - 1));
            Vector2Int nextMapPosition = Vector2Int.zero;
            float nextDistance = float.MaxValue;
            foreach (var position in frontier)
            {
                float tempDistance = Vector2.Distance(transform.position, new Vector2Int((int)Target.position.x, (int)Target.position.z));
                if (tempDistance < nextDistance || (tempDistance == nextDistance && UnityEngine.Random.Range(0, 1.0f) > 0.5f)) // Flip a coin for a tie.
                {
                    nextMapPosition = position;
                    nextDistance = tempDistance;
                }
            }
            if (nextMapPosition == Vector2Int.zero)
            {
                CurrentPath.Push(frontier[UnityEngine.Random.Range(0, frontier.Count)]);
            }
            CurrentPath.Push(nextMapPosition);
            return;
        }
        for (int i = pathTileTuples.Count - 1; i > 0; i--)
        {
            CurrentPath.Push(GetVectorFromCoordinatesTuple(pathTileTuples[i]));
        }
    }

    protected void SelectTarget()
    {
        switch (CurrentLogic)
        {
            case UnitLogic.Attack:
                if (Owner == Player.Human)
                {
                    Target = UnitHelpers.GetClosestNodeByPlayerOrNeutral(this.transform.position, Player.AI).transform;
                }
                else
                {
                    Target = UnitHelpers.GetClosestNodeByPlayerOrNeutral(this.transform.position, Player.Human).transform;
                }
                TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
                break;
            case UnitLogic.Defend:

                Target = UnitHelpers.GetNearestHostileUnit(this).transform;

                if (Vector3.Distance(this.transform.position, Target.transform.position) > defenseRadius)
                {
                    Target = null;
                }

                if (Target != null)
                {
                    TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
                }
                else
                {
                    var controlledNodes = DependencyService.Instance.DistrictController().GetNodesByPlayer(true);
                    Transform temporaryTarget = null;
                    float minimumDistance = float.MaxValue;
                    if (controlledNodes != null)
                    {
                        foreach (var node in controlledNodes)
                        {
                            float distance = Vector3.Distance(node.transform.position, transform.position);
                            if (distance < minimumDistance)
                            {
                                temporaryTarget = node.transform;
                                minimumDistance = distance;
                            }
                        }
                        Target = temporaryTarget;
                    }
                }
                break;

            case UnitLogic.Split:
            default:
                if (this.Owner == Player.Human)
                {
                    Target = UnitHelpers.GetRandomNodeByPlayerOrNeutral(Player.AI).transform;
                }
                else
                {
                    Target = UnitHelpers.GetRandomNodeByPlayerOrNeutral(Player.Human).transform;
                }
                TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
                break;

        }
    }
}
