using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Create a static reference to the instance
    private static LevelManager _instance;

    public static LevelManager Instance
    {
        get
        {
            // If there is no instance yet, find or create one
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();

                // If there are no instances in the scene, create a new one
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(LevelManager).Name);
                    _instance = singletonObject.AddComponent<LevelManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure there's only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Set the instance to this object
        _instance = this;

        // Make sure it persists between scenes
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);

        /*
        // Check if the scene exists in the build settings
        if (SceneExists(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("Scene '" + levelName + "' does not exist in the build settings.");
        }
        */
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private bool SceneExists(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid();
    }
}
