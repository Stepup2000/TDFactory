using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [Header("Effects to preload")]
    [SerializeField] private List<EffectData> effectsToPool;

    // Internal dictionary for effect pools using enums as keys
    private Dictionary<EffectType, Queue<BaseStatusEffect>> effectPools = new();

    // Optional parent transform for organizing pooled objects
    [SerializeField] private Transform poolParent;

    private static EffectController instance;

    /// <summary>
    /// Gets the singleton instance of the EffectController, creating one if it does not exist.
    /// </summary>
    public static EffectController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EffectController>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("EffectController");
                    instance = singletonObject.AddComponent<EffectController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        InitializePools();
    }

    /// <summary>
    /// Create the pools for all effects.
    /// </summary>
    private void InitializePools()
    {
        foreach (var data in effectsToPool)
        {
            var queue = new Queue<BaseStatusEffect>();

            for (int i = 0; i < data.initialPoolSize; i++)
            {
                BaseStatusEffect effect = Instantiate(data.effectPrefab, poolParent);
                effect.returnToPoolCallback = ReturnToPool;
                effect.OnDespawn();
                queue.Enqueue(effect);
            }

            // Store the pool with the corresponding enum key
            effectPools[data.effectType] = queue;
        }
    }

    /// <summary>
    /// Spawns and applies the effect to a target using an enum.
    /// </summary>
    /// <param name="effectType">The type of the effect.</param>
    /// <param name="target">Target to apply the effect to.</param>
    public void ApplyEffect(EffectType effectType, IDamageable target)
    {
        if (!effectPools.TryGetValue(effectType, out var pool))
        {
            Debug.LogWarning($"Effect '{effectType}' not found in pool.");
            return;
        }

        BaseStatusEffect effect = pool.Count > 0 ? pool.Dequeue() : Instantiate(GetPrefab(effectType), poolParent);
        effect.returnToPoolCallback = ReturnToPool;
        effect.OnSpawn();
        effect.ApplyEffect(target);

        if (target is Component targetComponent)
        {
            effect.transform.position = targetComponent.transform.position;
            effect.transform.SetParent(targetComponent.transform);
        }
            
    }

    /// <summary>
    /// Returns an effect back to its pool.
    /// </summary>
    private void ReturnToPool(BaseStatusEffect effect)
    {
        effect.OnDespawn(); // Cleanup effect

        // Assuming BaseStatusEffect has an 'EffectType' property
        if (effectPools.TryGetValue(effect.effectType, out var pool))  // Directly use effectType to find the correct pool
        {
            pool.Enqueue(effect); // Return effect to the pool
            effect.transform.SetParent(transform); // Set parent back to controller
        }
        else
        {
            // No pool for this effect type found; destroy the effect
            Debug.LogWarning($"Effect of type {effect.GetType()} was not part of any known pool.");
            Destroy(effect.gameObject);
        }
    }


    /// <summary>
    /// Gets the prefab associated with a given effect type.
    /// </summary>
    private BaseStatusEffect GetPrefab(EffectType effectType)
    {
        foreach (var data in effectsToPool)
        {
            if (data.effectType == effectType)
                return data.effectPrefab;
        }

        Debug.LogError($"No prefab registered for effect: {effectType}");
        return null;
    }
}
