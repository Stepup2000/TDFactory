using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, ISerializationCallbackReceiver
{
    //Declare delegate type for events that use an enemy type and damage
    public delegate void EnemyEvent(BaseEnemy enemy, float damage);
    public event EnemyEvent OnEnemyDetectedEvent;

    //Declare delegate type without parameters
    public delegate void TriggerEvent();
    public event TriggerEvent EnemyRequestEvent;

    //private serialized fields for stats
    [SerializeField] private List<string> _statKeys;
    [SerializeField] private List<float> _statValues;

    //Dictionary that hold all the stats of the tower
    private Dictionary<string, float> _stats;

    // Define the keys for your stats constants
    public const string PRICE_STAT = "Price";
    public const string DAMAGE_STAT = "BaseDamage";
    public const string RANGE_STAT = "BaseRange";
    public const string RELOADSPEED_STAT = "BaseReloadSpeed";

    //Hashset that holds all detectedEnemies
    private HashSet<BaseEnemy> _detectedEnemies = new();

    private BaseEnemy _lastEnemy = null;

    private void Awake()
    {
        // Initialize the stats dictionary with default values
        _stats = new Dictionary<string, float>()
        {
            { PRICE_STAT, 100f },
            { DAMAGE_STAT, 1f },
            { RANGE_STAT, 5f },
            { RELOADSPEED_STAT, 0.25f }
        };
        Debug.Log($"PRICE_STAT value: {GetStats(PRICE_STAT)}");
    }

    //End the FiringCoroutine
    private void OnDestroy()
    {
        StopCoroutine(FireRateCoroutine());
    }

    public void ActivateTower()
    {
        StartCoroutine(FireRateCoroutine());
    }

    //Get the current stats of the tower
    public float GetStats(string statName)
    {
        if (_stats.TryGetValue(statName, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found in tower: '{gameObject.name}'");
            return 0f;
        }
    }

    // Modify the stats of the tower based on the input modifiers dictionary.
    public void ModifyStats(Dictionary<string, float> modifiers)
    {
        //Returns if not modifiers are given
        if (modifiers == null)
        {
            Debug.LogWarning("Modifiers argument is null");
            return;
        }

        // If the new value is negative, clamps it to 0 and logs a warning.
        foreach (var kvp in modifiers)
        {
            if (_stats.TryGetValue(kvp.Key, out float oldValue))
            {
                float newValue = oldValue + kvp.Value;
                if (newValue < 0f)
                {
                    newValue = 0f;
                    Debug.LogWarning($"Stat value for '{kvp.Key}' cannot be negative. Clamping to 0.");
                }
                _stats[kvp.Key] = newValue;
            }
            else
            {
                Debug.LogWarning($"Invalid stat name: '{kvp.Key}'");
            }
        }
    }

    private IEnumerator FireRateCoroutine()
    {
        while (true)
        {
            // Request all enemies from detectors
            ObtainAllTargets();

            // Wait for the enemy request event to complete
            yield return new WaitForEndOfFrame();

            //Calculate the furthest target
            CalculateTarget();

            // Fire weapons
            TryFireWeapons();

            // Wait for reload time
            yield return new WaitForSeconds(_stats[RELOADSPEED_STAT]);
        }
    }

    //Method for DetectionModules to add their prefered enemytarget to the hashset of the tower
    public void AddTarget(BaseEnemy enemy)
    {
        if (_detectedEnemies != null && enemy != null) _detectedEnemies.Add(enemy);
    }

    //Gets rid of the old targets and sends out an event to get new targets
    private void ObtainAllTargets()
    {
        _detectedEnemies?.Clear();
        EnemyRequestEvent?.Invoke();
    }

    //Assigns a new target based on the distance they travelled, it wants the furthest one
    private void CalculateTarget()
    {
        BaseEnemy furthestEnemy = null;
        foreach (BaseEnemy enemy in _detectedEnemies)
        {
            if (enemy == null) continue;
            if (furthestEnemy == null || furthestEnemy.travelledDistance < enemy.travelledDistance) furthestEnemy = enemy;
        }
        _lastEnemy = furthestEnemy;
    }

    //Invokes an event if an enemy has been detected
    private void TryFireWeapons()
    {
        if (_detectedEnemies.Count != 0)
        {
            OnEnemyDetectedEvent?.Invoke(_lastEnemy, _stats[DAMAGE_STAT]);
        }        
    }

    #region ShowDictionary
    // This method is called before the object is serialized
    public void OnBeforeSerialize()
    {
        // Clear the lists before serializing
        _statKeys = new List<string>();
        _statValues = new List<float>();

        // Serialize each key-value pair to the lists
        foreach (var kvp in _stats)
        {
            _statKeys.Add(kvp.Key);
            _statValues.Add(kvp.Value);
        }
    }

    // This method is called after the object is deserialized
    public void OnAfterDeserialize()
    {
        // Create a new dictionary to store the deserialized key-value pairs
        _stats = new Dictionary<string, float>();

        // Deserialize each key-value pair from the lists
        for (int i = 0; i < _statKeys.Count; i++)
        {
            _stats[_statKeys[i]] = _statValues[i];
        }
    }
    #endregion
}