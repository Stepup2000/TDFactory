using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : MonoBehaviour, IDetectionModule
{    
    //All detected enemies
    private HashSet<BaseEnemy> _enemiesInRange = new();
    [SerializeField] private Tower _parentTower;

    //Subscribe to the EnemyRequestEvent
    private void OnEnable()
    {
        if (_parentTower != null) _parentTower.EnemyRequestEvent += SendFurthestEnemy;
    }

    //UnSubscribe to the EnemyRequestEvent
    private void OnDisable()
    {
        if (_parentTower != null) _parentTower.EnemyRequestEvent -= SendFurthestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseEnemy foundEnemy)) EnemyDetected(foundEnemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BaseEnemy foundEnemy)) EnemyLost(foundEnemy);
    }

    //Adds enemies to the hashset when in range
    public void EnemyDetected(BaseEnemy enemy)
    {
        _enemiesInRange.Add(enemy);
    }

    //Removes enemies 
    public void EnemyLost(BaseEnemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    //Get the enemy that travalled the most
    private BaseEnemy GetFurthestEnemy()
    {
        if (_enemiesInRange == null) return null;

        BaseEnemy furthestEnemy = null;
        float maxDistance = float.MinValue;     //Set maxDistance to MinValue so that the first one is always bigger

        foreach (BaseEnemy enemy in _enemiesInRange)
        {
            if (enemy == null) continue;

            float distance = enemy.travelledDistance;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestEnemy = enemy;
            }
        }
        return furthestEnemy;
    }

    //Sends the furthestenemy to the parent tower
    private void SendFurthestEnemy()
    {
        BaseEnemy enemy = GetFurthestEnemy();
        _parentTower.AddTarget(enemy);
    }
}
