using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a camera module for detecting enemies. Implements the IDetectionModule interface to handle enemy detection events.
/// </summary>
public class CameraModule : MonoBehaviour, IDetectionModule
{
    /// <summary>
    /// A set of detected enemies currently in range of the camera module.
    /// </summary>
    private HashSet<BaseEnemy> _enemiesInRange = new();

    /// <summary>
    /// Reference to the parent tower of this module.
    /// </summary>
    private Tower _parentTower;

    /// <summary>
    /// The cost of the module.
    /// </summary>
    [field: SerializeField] public int cost { get; set; }

    /// <summary>
    /// The prefab associated with this module.
    /// </summary>
    [field: SerializeField] public GameObject modulePrefab { get; set; }

    /// <summary>
    /// The sound clip played when the module is placed.
    /// </summary>
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    /// <summary>
    /// Called when the script is enabled. Subscribes to the EnemyRequestEvent and RequestModuleDataEvent.
    /// </summary>
    private void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the EnemyRequestEvent and RequestModuleDataEvent.
    /// </summary>
    private void OnDisable()
    {
        if (_parentTower != null) _parentTower.EnemyRequestEvent -= SendFurthestEnemy;
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    /// <summary>
    /// Handles the RequestModuleDataEvent by placing the module.
    /// </summary>
    /// <param name="requestModuleDataEvent">The event data.</param>
    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject.transform.parent.gameObject, modulePrefab);
    }

    /// <summary>
    /// Sets the parent tower of this module.
    /// </summary>
    /// <param name="newTower">The new parent tower.</param>
    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }

    /// <summary>
    /// Subscribes to the EnemyRequestEvent of the parent tower, if it is not null. Retries if the parent tower is not yet assigned.
    /// </summary>
    private void SetEnemyRequestSubscription()
    {
        if (_parentTower != null)
        {
            _parentTower.EnemyRequestEvent -= SendFurthestEnemy;
            _parentTower.EnemyRequestEvent += SendFurthestEnemy;
        }
        else Invoke("SetEnemyRequestSubscription", 0.5f);
    }

    /// <summary>
    /// Called when a collider enters the trigger. Detects and adds the enemy to the list of enemies in range.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseEnemy foundEnemy)) EnemyDetected(foundEnemy);
    }

    /// <summary>
    /// Called when a collider exits the trigger. Removes the enemy from the list of enemies in range.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BaseEnemy foundEnemy)) EnemyLost(foundEnemy);
    }

    /// <summary>
    /// Adds an enemy to the set of enemies in range.
    /// </summary>
    /// <param name="enemy">The detected enemy.</param>
    public void EnemyDetected(BaseEnemy enemy)
    {
        _enemiesInRange.Add(enemy);
    }

    /// <summary>
    /// Removes an enemy from the set of enemies in range.
    /// </summary>
    /// <param name="enemy">The lost enemy.</param>
    public void EnemyLost(BaseEnemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    /// <summary>
    /// Gets the enemy that has traveled the furthest distance from the set of enemies in range.
    /// </summary>
    /// <returns>The furthest enemy, or null if no enemies are in range.</returns>
    private BaseEnemy GetFurthestEnemy()
    {
        if (_enemiesInRange == null) return null;

        BaseEnemy furthestEnemy = null;
        float maxDistance = float.MinValue; // Set maxDistance to MinValue so that the first one is always bigger

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

    /// <summary>
    /// Sends the furthest enemy to the parent tower.
    /// </summary>
    private void SendFurthestEnemy()
    {
        BaseEnemy enemy = GetFurthestEnemy();
        _parentTower.AddTarget(enemy);
    }
}
