using Assets._Script.Units;
using System.Reflection;
using UnityEngine;
using static MapNodeController;

public class ProjectileShooter : PrefabSpawnAbility
{
    [SerializeField]
    private float maxDistanceAllowedToTarget = 2.0f;

    [SerializeField]
    private Transform target;

    [SerializeField]
    protected Player Owner;

    [SerializeField]
    protected bool CreateProjectilesWithoutTarget = false;
    protected override void SpawnGameObject(GameObject prefab)
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > maxDistanceAllowedToTarget)
        {
            var nearestEnemy = UnitHelpers.GetNearestHostileUnit(transform, Owner).transform;
            if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.transform.position) < maxDistanceAllowedToTarget)
            {
                target = nearestEnemy.transform;
            }
        }

        if (target != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Projectile>().Initialize(target.position, Owner);
        }
    }
}
