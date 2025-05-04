using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    [Header("Effects to preload")]
    [SerializeField] private List<EffectData> effectsToPool;

    private Dictionary<EffectType, Queue<BaseStatusEffect>> effectPools = new();

    private Dictionary<EffectType, BaseStatusEffect> prefabLookup = new();

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
                if (data.effectPrefab == null)
                {
                    Debug.LogError($"Effect prefab for type {data.effectType} is null.");
                    continue;
                }

                BaseStatusEffect effect = Instantiate(data.effectPrefab, poolParent);
                effect.returnToPoolCallback = ReturnToPool;
                effect.OnDespawn();
                queue.Enqueue(effect);
            }

            effectPools[data.effectType] = queue;

            if (!prefabLookup.ContainsKey(data.effectType))
                prefabLookup[data.effectType] = data.effectPrefab;
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

        BaseStatusEffect effect;

        if (pool.Count > 0)
        {
            effect = pool.Dequeue();
        }
        else
        {
            BaseStatusEffect prefab = GetPrefab(effectType);
            if (prefab == null) return;

            effect = Instantiate(prefab, poolParent);
        }

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
    /// <param name="effect">The effect to return.</param>
    private void ReturnToPool(BaseStatusEffect effect)
    {
        effect.OnDespawn();

        if (effectPools.TryGetValue(effect.effectType, out var pool))
        {
            pool.Enqueue(effect);
            effect.transform.SetParent(poolParent != null ? poolParent : transform);
        }
        else
        {
            Debug.LogWarning($"Effect of type {effect.GetType()} was not part of any known pool.");
            Destroy(effect.gameObject);
        }
    }

    /// <summary>
    /// Gets the prefab associated with a given effect type.
    /// </summary>
    /// <param name="effectType">The effect type to get the prefab for.</param>
    /// <returns>The corresponding BaseStatusEffect prefab.</returns>
    private BaseStatusEffect GetPrefab(EffectType effectType)
    {
        if (prefabLookup.TryGetValue(effectType, out var prefab))
            return prefab;

        Debug.LogError($"No prefab registered for effect: {effectType}");
        return null;
    }
}
