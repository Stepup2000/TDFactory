using UnityEngine;

/// <summary>
/// Interface for objects that can move along a path.
/// </summary>
public interface IMoveable
{
    /// <summary>
    /// Initializes the moveable object with a given path.
    /// </summary>
    /// <param name="path">An array of transforms representing the path to be followed.</param>
    void Initialize(Transform[] path);
}
