using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private EnemySpawner _spawnerPrefab;

    private Transform[] _myPath;

    private void Start()
    {
        _myPath = GetPath();
        SpawnEnemySpawner();
    }

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

    private void SpawnEnemySpawner()
    {
        if (_myPath != null && _spawnerPrefab != null)
        {
            EnemySpawner spawner = Instantiate<EnemySpawner>(_spawnerPrefab, transform.position, Quaternion.identity);
            spawner.Initialize(_myPath); 
        }
    }
}
