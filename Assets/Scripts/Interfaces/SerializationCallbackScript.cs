using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Demonstrates how to serialize and deserialize a Dictionary using Unity's serialization system.
/// </summary>
public class SerializationCallbackScript : MonoBehaviour, ISerializationCallbackReceiver
{
    /// <summary>
    /// List of keys used for serialization of the Dictionary.
    /// </summary>
    public List<int> _keys = new List<int> { 3, 4, 5 };

    /// <summary>
    /// List of values used for serialization of the Dictionary.
    /// </summary>
    public List<string> _values = new List<string> { "I", "Love", "Unity" };

    /// <summary>
    /// Dictionary to be serialized and deserialized.
    /// </summary>
    public Dictionary<int, string> _myDictionary = new Dictionary<int, string>();

    /// <summary>
    /// Called before serialization. Converts the Dictionary to lists of keys and values.
    /// </summary>
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in _myDictionary)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    /// <summary>
    /// Called after deserialization. Converts the lists of keys and values back to a Dictionary.
    /// </summary>
    public void OnAfterDeserialize()
    {
        _myDictionary = new Dictionary<int, string>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            _myDictionary.Add(_keys[i], _values[i]);
    }

    /// <summary>
    /// Displays the contents of the Dictionary in the GUI.
    /// </summary>
    void OnGUI()
    {
        foreach (var kvp in _myDictionary)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    }
}
