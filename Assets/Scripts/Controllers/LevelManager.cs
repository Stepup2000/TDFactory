using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level loading and scene transitions without needing an instance.
/// </summary>
public static class LevelManager
{
    /// <summary>
    /// Loads a scene by its name.
    /// </summary>
    /// <param name="levelName">Name of the scene to load.</param>
    public static void LoadLevel(string levelName)
    {
        if (levelName == "Quit") QuitGame();
        if (SceneExists(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("Scene '" + levelName + "' does not exist in the build settings.");
        }
    }

    /// <summary>
    /// Checks if a scene exists in the build settings.
    /// </summary>
    /// <param name="sceneName">Name of the scene to check.</param>
    /// <returns>True if the scene is valid, otherwise false.</returns>
    private static bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Exits the application or stops play mode in the Unity Editor.
    /// </summary>
    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
