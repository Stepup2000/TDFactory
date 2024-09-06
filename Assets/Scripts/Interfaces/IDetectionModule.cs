using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for modules that detect enemies.
/// </summary>
public interface IDetectionModule : IModule
{
    /// <summary>
    /// Called when an enemy is detected by the module.
    /// </summary>
    /// <param name="enemy">The enemy that was detected.</param>
    void EnemyDetected(BaseEnemy enemy);

    /// <summary>
    /// Called when an enemy is no longer detected by the module.
    /// </summary>
    /// <param name="enemy">The enemy that was lost.</param>
    void EnemyLost(BaseEnemy enemy);
}
