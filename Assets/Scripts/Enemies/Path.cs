using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the path for enemy spawners in the game.
/// </summary>
public class Path : MonoBehaviour
{
    [SerializeField] private EnemySpawner _spawnerPrefab; // Prefab used to instantiate enemy spawners

    private Transform[] _myPath; // Array of waypoints defining the path

    /// <summary>
    /// Initializes the path by retrieving the waypoints and spawning the enemy spawner.
    /// </summary>
    private void Start()
    {
        _myPath = GetPath();
        SpawnEnemySpawner();
    }

    /// <summary>
    /// Retrieves all child transforms of this GameObject to define the path.
    /// </summary>
    /// <returns>An array of Transforms representing the path waypoints, or null if no children exist.</returns>
    private Transform[] GetPath()
    {
        if (transform.childCount == 0) return null;

        Transform[] newPoints = new Transform[transform.childCount];
        for (int i = 0; i < newPoints.Length; i++)
        {
            newPoints[i] = transform.GetChild(i);
        }

        return newPoints;
    }

    /// <summary>
    /// Instantiates an enemy spawner at the path's position and initializes it with the path waypoints.
    /// </summary>
    private void SpawnEnemySpawner()
    {
        if (_myPath != null && _spawnerPrefab != null)
        {
            EnemySpawner spawner = Instantiate<EnemySpawner>(_spawnerPrefab, transform.position, Quaternion.identity);
            spawner.Initialize(_myPath);
        }
    }
}
