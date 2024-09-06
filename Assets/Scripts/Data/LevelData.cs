using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject for storing level data. Contains information about enemy prefabs, waves, starting resources, and enemy budgets.
/// </summary>
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    /// <summary>
    /// Array of enemy prefabs available for the level.
    /// </summary>
    public BaseEnemy[] availableEnemyPrefabs;

    /// <summary>
    /// The total number of waves in the level.
    /// </summary>
    public int AmountOfWaves = 25;

    /// <summary>
    /// The amount of starting currency for the level.
    /// </summary>
    public int StartingCurrency = 100;

    /// <summary>
    /// The amount of starting health for the level.
    /// </summary>
    public int StartingHealth = 10;

    /// <summary>
    /// The initial budget allocated for enemies in each wave.
    /// </summary>
    public int EnemyStartingBudget = 100;

    /// <summary>
    /// Multiplier applied to the enemy budget for each subsequent wave.
    /// </summary>
    public float EnemyBudgetMultiplier = 1.1f;
}
