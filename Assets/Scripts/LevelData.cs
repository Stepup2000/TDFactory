using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object for wavedata, waves are made out of this
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public BaseEnemy[] availableEnemyPrefabs;
    public int AmountOfWaves = 25;
    public int StartingCurrency = 100;
    public int StartingHealth = 10;
    public int EnemyStartingBudget = 100;
    public float EnemyBudgetMultiplier = 1.1f;
}