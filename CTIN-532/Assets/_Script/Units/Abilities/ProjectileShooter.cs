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
    Player owner;

    private GameManager gameManager;

    private void FindGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    protected override void HandleSpawnedGameObject(GameObject spawnedGameObject)
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > maxDistanceAllowedToTarget)
        {
            if (gameManager == null)
            {
                FindGameManager();
            }
            if (gameManager == null)
            {
                Destroy(spawnedGameObject);
                return;
            }
            var nearestEnemy = gameManager.GetClosestUnitByPlayer(transform.position, owner);
            if (nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.transform.position) > maxDistanceAllowedToTarget)
            {
                target = nearestEnemy.transform;
            }
        }

        if (target != null)
        {
            if (!spawnedGameObject.TryGetComponent<Projectile>(out var projectile))
            {
                Debug.LogError("The spawned object was not a projectile.");
                Destroy(gameObject);
            }
            else
            {
                projectile.SetTarget(target.position);
            }
        }
        else
        {
            Destroy(spawnedGameObject);
        }
    }
}
