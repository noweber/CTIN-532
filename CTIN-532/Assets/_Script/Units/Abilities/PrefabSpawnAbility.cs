using UnityEngine;

public abstract class PrefabSpawnAbility : UnitAbility
{
    [SerializeField]
    private GameObject prefabToSpawnWhenAbilityIsUsed;

    protected override void UseAbility()
    {
        SpawnGameObject(prefabToSpawnWhenAbilityIsUsed);

    }

    protected abstract void SpawnGameObject(GameObject prefab);
}
