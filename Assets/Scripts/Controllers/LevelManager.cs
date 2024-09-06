using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level loading, scene transitions, and background music.
/// Implements a singleton pattern to ensure only one instance exists and persists between scenes.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusicClip; // Clip for background music

    private static LevelManager _instance; // Singleton instance of LevelManager

    /// <summary>
    /// Provides access to the singleton instance of LevelManager.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(LevelManager).Name);
                    _instance = singletonObject.AddComponent<LevelManager>();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Ensures there's only one instance of LevelManager and makes it persist between scenes.
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Plays the background music clip at the start of the scene.
    /// </summary>
    private void Start()
    {
        SoundManager.Instance.PlaySoundAtLocation(backgroundMusicClip, transform.position, false, true);
    }

    /// <summary>
    /// Loads a scene by its name.
    /// </summary>
    /// <param name="levelName">Name of the scene to load.</param>
    public void LoadLevel(string levelName)
    {
        // Code for checking if the scene exists, currently does not work.
        /*
        if (SceneExists(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("Scene '" + levelName + "' does not exist in the build settings.");
        }
        */

        SceneManager.LoadScene(levelName);
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Checks if a scene exists in the build settings.
    /// </summary>
    /// <param name="sceneName">Name of the scene to check.</param>
    /// <returns>True if the scene is valid, otherwise false.</returns>
    private bool SceneExists(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid();
    }
}
