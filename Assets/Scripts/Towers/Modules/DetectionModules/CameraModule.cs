using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : MonoBehaviour, IDetectionModule
{
    //All detected enemies
    private HashSet<BaseEnemy> _enemiesInRange = new();
    private Tower _parentTower;
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    //Subscribe to the EnemyRequestEvent
    private void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    //UnSubscribe to the EnemyRequestEvent
    private void OnDisable()
    {
        if (_parentTower != null) _parentTower.EnemyRequestEvent -= SendFurthestEnemy;
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject.transform.parent.gameObject, modulePrefab);
    }

    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }

    private void SetEnemyRequestSubscription()
    {
        if (_parentTower != null)
        {
            _parentTower.EnemyRequestEvent -= SendFurthestEnemy;
            _parentTower.EnemyRequestEvent += SendFurthestEnemy;
        }
        else Invoke("SetEnemyRequestSubscription", 0.5f);
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
