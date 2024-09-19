using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages scene initialization and publishes events to set up the level based on provided data.
/// Implements a singleton pattern to ensure only one instance exists and persists across scenes.
/// </summary>
public class SceneController : MonoBehaviour
{
    [SerializeField] private LevelData _levelData; // Reference to the level data used for initialization

    private static SceneController _instance; // Singleton instance of SceneController

    /// <summary>
    /// Provides access to the singleton instance of SceneController.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneController>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SceneController).Name);
                    _instance = singletonObject.AddComponent<SceneController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes the level shortly after the scene starts by invoking the InitializeLevel method.
    /// </summary>
    private void Start()
    {
        Invoke("InitializeLevel", 0.1f);
    }

    /// <summary>
    /// Publishes events to initialize the level with the data provided in _levelData.
    /// Sets the starting currency and health based on the level data.
    /// </summary>
    private void InitializeLevel()
    {
        if (_levelData != null)
        {
            EventBus<InitializeLevel>.Publish(new InitializeLevel(_levelData));
            EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(_levelData.StartingCurrency, Vector3.zero));
            EventBus<ChangeHealthEvent>.Publish(new ChangeHealthEvent(_levelData.StartingHealth));
        }
    }
}
