public interface IPoolableEffect
{
    /// <summary>
    /// Method called when the effects spawns in.
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// Method called when the effect ends.
    /// </summary>
    void OnDespawn();
}
