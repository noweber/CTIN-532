using System.Reflection;
using UnityEngine;

public abstract class PrefabSpawnAbility : UnitAbility
{
    [SerializeField]
    private GameObject prefabToSpawnWhenAbilityIsUsed;

    protected override void UseAbility()
    {
        Debug.Log(MethodBase.GetCurrentMethod());
        var spawnedGameObject = Instantiate(prefabToSpawnWhenAbilityIsUsed, transform.position, Quaternion.identity);
        HandleSpawnedGameObject(spawnedGameObject);
    }

    protected abstract void HandleSpawnedGameObject(GameObject spawnedGameObject);
}
