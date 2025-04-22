using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObjectPooler<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int initialPoolSize = 10;
    [SerializeField] protected Transform poolParent;

    protected readonly List<T> pool = new();

    protected virtual void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError($"Prefab is not assigned in {GetType().Name}. Disabling component.");
            enabled = false;
            return;
        }

        InitializePool();
    }

    /// <summary>
    /// Creates the initial pool.
    /// </summary>
    protected virtual void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            T obj = CreateNewInstance();
            obj.OnDespawn();
            pool.Add(obj);
        }
    }

    /// <summary>
    /// Spawns or reuses an object from the pool.
    /// </summary>
    public virtual T GetFromPool()
    {
        foreach (var obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.ResetObject();
                obj.OnSpawn();
                return obj;
            }
        }

        // Expand pool if none available
        T newObj = CreateNewInstance();
        newObj.OnSpawn();
        pool.Add(newObj);
        Debug.Log($"[{GetType().Name}] Pool expanded. New size: {pool.Count}");
        return newObj;
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// </summary>
    public virtual void ReturnToPool(T obj)
    {
        obj.OnDespawn();
        obj.transform.SetParent(poolParent != null ? poolParent : transform);
        obj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Instantiates and prepares a new instance.
    /// </summary>
    protected T CreateNewInstance()
    {
        GameObject obj = Instantiate(prefab, poolParent);
        obj.SetActive(false);

        if (!obj.TryGetComponent(out T poolableComponent))
        {
            Debug.LogError($"Prefab does not have a component of type {typeof(T).Name}");
        }

        return poolableComponent;
    }
}
