public interface IPoolable
{
    /// <summary>
    /// Called when the object is taken from the pool.
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// Called when the object is returned to the pool.
    /// </summary>
    void OnDespawn();

    /// <summary>
    /// Called to reset the object's internal state before reuse.
    /// </summary>
    void ResetObject();
}

