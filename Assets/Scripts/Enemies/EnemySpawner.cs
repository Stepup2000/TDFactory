using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of enemies at a set interval and follows a given path.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnSpeed = 5f; // The rate at which enemies will be spawned
    private Transform[] _myPath; // The path that spawned enemies will follow
    private Queue<BaseEnemy> _spawnQueue = new Queue<BaseEnemy>(); // Queue of enemies to be spawned

    private void OnEnable()
    {
        // Subscribe to the SpawnEnemyEvent to queue enemies for spawning
        EventBus<SpawnEnemyEvent>.Subscribe(QueueEnemiesToSpawn);
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private void OnDisable()
    {
        // Unsubscribe from the SpawnEnemyEvent and stop the spawning coroutine
        EventBus<SpawnEnemyEvent>.UnSubscribe(QueueEnemiesToSpawn);
        StopCoroutine(SpawnEnemyCoroutine());
    }

    /// <summary>
    /// Initializes the spawner with a path for the enemies to follow.
    /// </summary>
    /// <param name="path">Array of waypoints for the enemies to follow.</param>
    public void Initialize(Transform[] path)
    {
        _myPath = path;
    }

    /// <summary>
    /// Adds enemies to the spawn queue.
    /// </summary>
    /// <param name="pEvent">Event containing the enemies to spawn.</param>
    private void QueueEnemiesToSpawn(SpawnEnemyEvent pEvent)
    {
        foreach (BaseEnemy enemy in pEvent.enemies)
        {
            _spawnQueue.Enqueue(enemy);
        }
    }

    /// <summary>
    /// Coroutine that spawns enemies at the specified interval.
    /// </summary>
    /// <returns>Enumerator for the coroutine.</returns>
    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            if (_spawnQueue.Count > 0)
            {
                BaseEnemy enemy = _spawnQueue.Peek();
                TrySpawnEnemy(enemy);
            }
            yield return new WaitForSeconds(_spawnSpeed);
        }
    }

    /// <summary>
    /// Attempts to spawn the given enemy if conditions are met.
    /// </summary>
    /// <param name="pEnemy">Enemy to spawn.</param>
    private void TrySpawnEnemy(BaseEnemy pEnemy)
    {
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

    /// <summary>
    /// Creates and spawns an enemy at the start of the path.
    /// </summary>
    /// <param name="enemyPrefab">Enemy prefab to instantiate.</param>
    private void CreateEnemy(BaseEnemy enemyPrefab)
    {
        Vector3 spawnPosition = _myPath[0].position;
        BaseEnemy createdEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        createdEnemy.Initialize(_myPath);
        createdEnemy.transform.SetParent(GameObject.Find("EnemiesCanvas").transform);
    }
}
