using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages scene loading by providing a method to load levels through the LevelManager.
/// </summary>
public class SceneHelper : MonoBehaviour
{
    /// <summary>
    /// Loads the specified scene using the LevelManager.
    /// This method can be called from UI buttons or other scripts.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadLevel(string sceneName)
    {
        LevelManager.LoadLevel(sceneName);
    }
}
