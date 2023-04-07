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

    private GameManager gameManager;

    private void FindGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    protected override void SpawnGameObject(GameObject prefab)
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > maxDistanceAllowedToTarget)
        {
            if (gameManager == null)
            {
                FindGameManager();
            }
            if (gameManager == null)
            {
                Debug.LogWarning("Could not find game manager.");
                return;
            }
            Player enemies = Player.Human;
            if (Owner == Player.Human)
            {
                enemies = Player.AI;
            }
            var nearestEnemy = gameManager.GetClosestUnitByPlayer(transform.position, enemies);
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
