using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetectionModule : IModule
{
    void EnemyDetected(BaseEnemy enemy);
    void EnemyLost(BaseEnemy enemy);
}
