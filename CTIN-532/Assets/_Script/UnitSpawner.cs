using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject spawnPrefab;

    public float minSecondsBetweenSpawning = 1.0f;

    public float maxSecondsBetweenSpawning = 4.0f;

    private float secondsElapsedSinceLastSpawn;

    private float secondsUntilNextSpawn;

    private int spawnCount;

    // Use this for initialization
    void Start()
    {
        this.secondsElapsedSinceLastSpawn = 1.0f;
        this.secondsElapsedSinceLastSpawn = 0;
        this.spawnCount = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if (this.spawnCount >= 15)
        {
            return;
        }*/

        if (this.spawnPrefab == null)
        {
            Debug.Log("Spawn prefab is null");
            return;
        }

        this.secondsElapsedSinceLastSpawn += Time.deltaTime;
        if (this.secondsElapsedSinceLastSpawn > this.secondsUntilNextSpawn)
        {
            this.ResetSpawnTimer();
            GameObject spawn = Instantiate(this.spawnPrefab);
            spawn.transform.position = this.transform.position;
            spawnCount++;
        }
    }

    private void ResetSpawnTimer()
    {
        this.secondsElapsedSinceLastSpawn = 0;
        this.secondsUntilNextSpawn = Random.Range(minSecondsBetweenSpawning, maxSecondsBetweenSpawning);
    }
}
