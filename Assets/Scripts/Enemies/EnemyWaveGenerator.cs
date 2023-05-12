using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyWaveGenerator
{
    // The maximum percentage of the enemyWaveBudget that an enemy can cost
    private static int maxAffordableEnemyPercentage = 10;

    public static BaseEnemy[] GenerateEnemyWave(BaseEnemy[] potentialEnemies, int enemyWaveBudget)
    {
        List<BaseEnemy> eligibleEnemies = new();
        // Determine which enemies are eligible based on their spawnCost and the budget
        foreach (BaseEnemy enemy in potentialEnemies)
        {
            float maxCost = enemyWaveBudget * maxAffordableEnemyPercentage / 100;
            if (enemy.spawnCost <= maxCost)
            {
                eligibleEnemies.Add(enemy);
            }
        }

        List<BaseEnemy> chosenEnemies = new();
        // If there are eligible enemies, choose which ones to include in the wave
        if (eligibleEnemies.Count > 0)
        {
            // Sort the eligible enemies by spawnCost to ensure we can break out of the loop early
            // when remainingCurrency is less than the cheapest eligible enemy's spawnCost.
            eligibleEnemies.Sort((a, b) => a.spawnCost.CompareTo(b.spawnCost));
            int remainingCurrency = enemyWaveBudget;
            while (remainingCurrency >= eligibleEnemies[0].spawnCost)
            {
                // Choose a random eligible enemy to add to the wave
                BaseEnemy enemyToAdd = eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];
                if (enemyToAdd.spawnCost <= remainingCurrency)
                {
                    // Add the enemy to the wave and subtract its cost from the remaining budget
                    chosenEnemies.Add(enemyToAdd);
                    remainingCurrency -= enemyToAdd.spawnCost;
                }
            }
        }
        else Debug.LogWarning("There were no eligible enemies, all too expensive");

        if (chosenEnemies == null || chosenEnemies.Count == 0)
        {
            Debug.LogWarning("noone chosen");
            return null;
        }
        else return chosenEnemies.ToArray();
    }
}
