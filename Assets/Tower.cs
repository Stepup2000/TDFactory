using System;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> _statKeys;
    [SerializeField] private List<float> _statValues;

    private Dictionary<string, float> _stats;

    // Define the keys for your stats constants
    public const string DAMAGE_STAT = "BaseDamage";
    public const string RANGE_STAT = "BaseRange";
    public const string RELOADSPEED_STAT = "BaseReloadSpeed";

    private void Awake()
    {
        // Initialize the stats dictionary with default values
        _stats = new Dictionary<string, float>()
        {
            { DAMAGE_STAT, 1f },
            { RANGE_STAT, 5f },
            { RELOADSPEED_STAT, 2f }
        };
    }

    //Get the current stats of the tower
    public Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>(_stats);
    }

    public void ModifyStats(Dictionary<string, float> modifiers)
    {
        // Changes stats based on a Dictionary, the key is the stat name, the value is the amount it will change
        foreach (var modifier in modifiers)
        {
            if (_stats.ContainsKey(modifier.Key))
            {
                _stats[modifier.Key] += modifier.Value;
            }
            else
            {
                Debug.LogError($"Invalid stat name: {modifier.Key}");
            }
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