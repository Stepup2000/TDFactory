using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds data for a specific status effect type, including its prefab and initial pool size.
/// Used to set up the effect pooling system in the EffectController.
/// </summary>
[System.Serializable]
public class EffectData
{
    /// <summary>
    /// The type of the status effect (e.g., Fire, Ice, Poison).
    /// </summary>
    public EffectType effectType;

    /// <summary>
    /// The prefab representing the effect to be spawned and reused.
    /// </summary>
    public BaseStatusEffect effectPrefab;

    /// <summary>
    /// The number of instances to initially create in the pool.
    /// </summary>
    public int initialPoolSize;
}
