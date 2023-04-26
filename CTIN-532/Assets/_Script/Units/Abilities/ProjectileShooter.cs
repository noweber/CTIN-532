using Assets._Script.Units;
using System.Reflection;
using UnityEngine;
using static MapNodeController;

public class ProjectileShooter : PrefabSpawnAbility
{
    [SerializeField]
    public float MaxDistanceAllowedToTarget = 2.0f;

    [SerializeField]
    public Transform Target { get; private set; }

    [SerializeField]
    protected Player Owner;

    [SerializeField]
    protected bool CreateProjectilesWithoutTarget = false;

    protected override void SpawnGameObject(GameObject prefab)
    {
        if (Target == null || Vector3.Distance(transform.position, Target.position) > MaxDistanceAllowedToTarget)
        {
            var nearestEnemy = UnitHelpers.GetNearestHostileUnit(transform, Owner).transform;
            if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.transform.position) < MaxDistanceAllowedToTarget)
            {
                Target = nearestEnemy.transform;
            }
        }

        if (Target != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Projectile>().Initialize(Target.position, Owner);
        }
    }
}
