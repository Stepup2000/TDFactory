using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // The rate at which enemies will be spawned
    [SerializeField] private float _spawnSpeed = 5;
    // The path that spawned enemies will follow
    private Transform[] _myPath;
    // Queue of enemies to be spawned
    private Queue<BaseEnemy> _spawnQueue = new();

    private void OnEnable()
    {
        // Subscribe to the SpawnEnemyEvent so we can queue enemies for spawning
        EventBus<SpawnEnemyEvent>.Subscribe(QueueEnemiesToSpawn);
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private void OnDisable()
    {
        // Unsubscribe from the SpawnEnemyEvent and stop the spawning coroutine
        EventBus<SpawnEnemyEvent>.UnSubscribe(QueueEnemiesToSpawn);
        StopCoroutine(SpawnEnemyCoroutine());
    }

    public void Initialize(Transform[] path)
    {
        // Set the path that spawned enemies will follow
        if (_myPath == null) _myPath = path;
    }

    private void QueueEnemiesToSpawn(SpawnEnemyEvent pEvent)
    {
        // Add each enemy in the event to the spawn queue
        foreach (BaseEnemy enemy in pEvent.enemies)
        {
            _spawnQueue.Enqueue(enemy);
        }
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            // If there are enemies in the spawn queue, try to spawn the first one and wait for the specified delay
            if (_spawnQueue != null && _spawnQueue.Count > 0)
            {
                BaseEnemy enemy = _spawnQueue.Peek();
                TrySpawnEnemy(enemy);
            }
            yield return new WaitForSeconds(_spawnSpeed);
        }
    }

    private void TrySpawnEnemy(BaseEnemy pEnemy)
    {
        // If there is a path, a valid enemy, and the path has at least one waypoint, create and spawn the enemy
        if (_myPath != null && pEnemy != null && _myPath.Length > 0)
        {            
            CreateEnemy(pEnemy);
            _spawnQueue.Dequeue();
            if (_spawnQueue.Count == 0)
            {
                StopCoroutine(SpawnEnemyCoroutine());
                EventBus<StoppedSpawningEvent>.Publish(new StoppedSpawningEvent());                
            }
        }
    }

    private void CreateEnemy(BaseEnemy enemyPrefab)
    {
        // Set the spawn position for the enemy to be the first waypoint in the path
        Vector3 newPosition = _myPath[0].position;
        // Create the enemy and set its path
        BaseEnemy createdEnemy = Instantiate<BaseEnemy>(enemyPrefab, newPosition, Quaternion.identity);
        createdEnemy.Initialize(_myPath);
        createdEnemy.transform.SetParent(GameObject.Find("EnemiesCanvas").transform);
    }
}
