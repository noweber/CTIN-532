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
                Debug.LogWarning("Could not find game manager.");
                Destroy(spawnedGameObject);
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
            if (!spawnedGameObject.TryGetComponent<Projectile>(out var projectile))
            {
                Debug.LogError("The spawned object was not a projectile.");
                Destroy(spawnedGameObject);
            }
            else
            {
                projectile.SetTarget(target.position);
                projectile.Owner = Owner;
                projectile.IsArmed = true;
            }
        }
    }
}
