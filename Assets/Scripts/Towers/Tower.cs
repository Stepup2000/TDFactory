using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, ISerializationCallbackReceiver
{
    /// <summary>
    /// Delegate for handling enemy detection events, passing the detected enemy and damage value.
    /// </summary>
    /// <param name="enemy">The detected enemy.</param>
    /// <param name="damage">The damage dealt to the enemy.</param>
    public delegate void EnemyEvent(BaseEnemy enemy, float damage);

    /// <summary>
    /// Event triggered when an enemy is detected.
    /// </summary>
    public event EnemyEvent OnEnemyDetectedEvent;

    /// <summary>
    /// Delegate for triggering generic events without parameters.
    /// </summary>
    public delegate void TriggerEvent();

    /// <summary>
    /// Event triggered to request enemy targets from attached detection modules.
    /// </summary>
    public event TriggerEvent EnemyRequestEvent;

    [SerializeField] private List<string> _statKeys;
    [SerializeField] private List<float> _statValues;

    private Dictionary<string, float> _stats;

    // Constants defining the keys for tower stats.
    public const string PRICE_STAT = "Price";
    public const string DAMAGE_STAT = "BaseDamage";
    public const string RANGE_STAT = "BaseRange";
    public const string RELOADSPEED_STAT = "BaseReloadSpeed";

    private HashSet<BaseEnemy> _detectedEnemies = new();
    private BaseEnemy _lastEnemy = null;

    /// <summary>
    /// Initializes default stats when the tower is instantiated.
    /// </summary>
    private void Awake()
    {
        // Initialize the stats dictionary with default values.
        _stats = new Dictionary<string, float>()
        {
            { PRICE_STAT, 100f },
            { DAMAGE_STAT, 1f },
            { RANGE_STAT, 5f },
            { RELOADSPEED_STAT, 0.25f }
        };
    }

    /// <summary>
    /// Stops the firing coroutine when the tower is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        StopCoroutine(FireRateCoroutine());
    }

    /// <summary>
    /// Activates the tower by starting the firing coroutine.
    /// </summary>
    public void ActivateTower()
    {
        StartCoroutine(FireRateCoroutine());
    }

    /// <summary>
    /// Retrieves the current value of a specified stat.
    /// </summary>
    /// <param name="statName">The name of the stat to retrieve.</param>
    /// <returns>The value of the stat, or 0 if not found.</returns>
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

    /// <summary>
    /// Modifies the stats of the tower based on the given dictionary of modifiers.
    /// </summary>
    /// <param name="modifiers">A dictionary containing stat modifications.</param>
    public void ModifyStats(Dictionary<string, float> modifiers)
    {
        if (modifiers == null)
        {
            Debug.LogWarning("Modifiers argument is null");
            return;
        }

        // Apply each modifier, clamping values to 0 if necessary.
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

    /// <summary>
    /// Coroutine that handles the tower's firing cycle.
    /// </summary>
    private IEnumerator FireRateCoroutine()
    {
        while (true)
        {
            ObtainAllTargets();

            yield return new WaitForEndOfFrame();

            CalculateTarget();
            TryFireWeapons(false);

            // Wait for the tower's reload time before firing again.
            yield return new WaitForSeconds(_stats[RELOADSPEED_STAT]);
        }
    }

    /// <summary>
    /// Adds an enemy to the set of detected enemies.
    /// </summary>
    /// <param name="enemy">The enemy to add.</param>
    public void AddTarget(BaseEnemy enemy)
    {
        if (_detectedEnemies != null && enemy != null)
            _detectedEnemies.Add(enemy);
    }

    /// <summary>
    /// Clears current targets and requests new ones from detection modules.
    /// </summary>
    private void ObtainAllTargets()
    {
        _detectedEnemies?.Clear();
        EnemyRequestEvent?.Invoke();
    }

    /// <summary>
    /// Determines the furthest enemy in the set of detected enemies and assigns it as the target.
    /// </summary>
    private void CalculateTarget()
    {
        BaseEnemy furthestEnemy = null;
        foreach (BaseEnemy enemy in _detectedEnemies)
        {
            if (enemy == null) continue;
            if (furthestEnemy == null || furthestEnemy.travelledDistance < enemy.travelledDistance)
                furthestEnemy = enemy;
        }
        _lastEnemy = furthestEnemy;
    }

    /// <summary>
    /// Attempts to fire at the selected enemy if any enemies are detected.
    /// </summary>
    public void TryFireWeapons(bool overrideFiremechanism)
    {
        if (overrideFiremechanism)
        {
            OnEnemyDetectedEvent?.Invoke(null, _stats[DAMAGE_STAT]);
            return;
        }
        else if (_detectedEnemies.Count != 0)
            OnEnemyDetectedEvent?.Invoke(_lastEnemy, _stats[DAMAGE_STAT]);
    }

    #region ShowDictionary
    /// <summary>
    /// Serializes the tower's stats into lists for saving.
    /// </summary>
    public void OnBeforeSerialize()
    {
        // Clear the lists before serializing.
        _statKeys = new List<string>();
        _statValues = new List<float>();

        // Add each key-value pair from the dictionary to the lists.
        foreach (var kvp in _stats)
        {
            _statKeys.Add(kvp.Key);
            _statValues.Add(kvp.Value);
        }
    }

    /// <summary>
    /// Deserializes the tower's stats from the saved lists.
    /// </summary>
    public void OnAfterDeserialize()
    {
        // Initialize the stats dictionary from the deserialized lists.
        _stats = new Dictionary<string, float>();

        // Populate the dictionary with key-value pairs from the lists.
        for (int i = 0; i < _statKeys.Count; i++)
        {
            _stats[_statKeys[i]] = _statValues[i];
        }
    }
    #endregion
}
