using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the wave system for the game, including spawning enemies and handling wave transitions.
/// Implements a singleton pattern to ensure only one instance of the controller exists.
/// </summary>
public class WaveController : MonoBehaviour
{
    private LevelData _levelData; // Holds the level configuration data.
    private enum WaveState { Start, Spawning, Fighting, End } // Enumeration for different states of a wave.
    private WaveState waveState = WaveState.Start; // Current state of the wave.

    private int _waveIndex = 1; // Index of the current wave.

    private static WaveController instance; // Singleton instance of the WaveController.

    /// <summary>
    /// Gets the singleton instance of the WaveController, creating one if it does not exist.
    /// </summary>
    public static WaveController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WaveController>();
                if (instance == null)
                {
                    // Create a new GameObject with WaveController component if none exists
                    GameObject singletonObject = new GameObject("WaveController");
                    instance = singletonObject.AddComponent<WaveController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Subscribes to events when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        EventBus<StoppedSpawningEvent>.Subscribe(SetWaveToFighting);
        EventBus<InitializeLevel>.Subscribe(StartGameLoop);
    }

    /// <summary>
    /// Unsubscribes from events and stops the game loop coroutine when the script is disabled.
    /// </summary>
    private void OnDisable()
    {
        EventBus<StoppedSpawningEvent>.UnSubscribe(SetWaveToFighting);
        StopCoroutine(GameLoop()); // Stop the game loop coroutine
        EventBus<InitializeLevel>.UnSubscribe(StartGameLoop);
    }

    /// <summary>
    /// Starts the game loop coroutine and validates level data.
    /// </summary>
    /// <param name="pEvent">Event containing the level data.</param>
    private void StartGameLoop(InitializeLevel pEvent)
    {
        ValidateLevelData(pEvent); // Validate the level data
        if (_levelData != null) StartCoroutine(GameLoop());
        else Debug.LogWarning("No LevelData has been assigned in the controller.");
    }

    /// <summary>
    /// Validates the provided level data to ensure all required fields are set.
    /// </summary>
    /// <param name="pEvent">Event containing the level data.</param>
    private void ValidateLevelData(InitializeLevel pEvent)
    {
        _levelData = pEvent.data;
        // Log warnings if any level data fields are not set correctly
        if (_levelData == null) Debug.LogWarning("No LevelData loaded.");
        if (_levelData.availableEnemyPrefabs == null || _levelData.availableEnemyPrefabs.Length <= 0)
            Debug.LogWarning("No EnemyPrefabs loaded.");
        if (_levelData.AmountOfWaves <= 0) Debug.LogWarning("WaveAmount is too low.");
        if (_levelData.StartingHealth <= 0) Debug.LogWarning("StartingHealth is too low.");
        if (_levelData.StartingCurrency < 100) Debug.LogWarning("StartingCurrency is below 100.");
        if (_levelData.EnemyStartingBudget < 10) Debug.LogWarning("EnemyStartingBudget is below 10.");
        if (_levelData.EnemyBudgetMultiplier < 1) Debug.LogWarning("EnemyBudgetMultiplier is below 1.");
    }

    /// <summary>
    /// Coroutine that manages the game loop, handling wave transitions and enemy spawning.
    /// </summary>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator GameLoop()
    {
        while (true)
        {
            switch (waveState)
            {
                case WaveState.Start:
                    SpawnWave(); // Start spawning a new wave of enemies
                    waveState = WaveState.Spawning;
                    EventBus<WaveStarted>.Publish(new WaveStarted(_waveIndex)); // Notify that a new wave has started
                    break;

                case WaveState.Spawning:
                    // Wait until spawning is complete
                    yield return new WaitForSeconds(1f);
                    break;

                case WaveState.Fighting:
                    // Check if all enemies are defeated
                    if (AreAllEnemiesDefeated())
                    {
                        waveState = WaveState.End;
                    }
                    else yield return new WaitForSeconds(1f);
                    break;

                case WaveState.End:
                    IncreaseWave(); // Increase wave index and prepare for the next wave
                    waveState = WaveState.Start;
                    break;
            }
        }
    }

    /// <summary>
    /// Changes the wave state to Fighting when spawning is complete.
    /// </summary>
    /// <param name="pEvent">Event indicating that spawning has stopped.</param>
    private void SetWaveToFighting(StoppedSpawningEvent pEvent)
    {
        if (waveState == WaveState.Spawning) waveState = WaveState.Fighting;
    }

    /// <summary>
    /// Checks if all enemies have been defeated.
    /// </summary>
    /// <returns>True if all enemies are defeated, otherwise false.</returns>
    private bool AreAllEnemiesDefeated()
    {
        // Check if there are any active enemies
        return FindObjectOfType<BaseEnemy>() == null;
    }

    /// <summary>
    /// Increases the wave index and checks if the game has been won.
    /// </summary>
    private void IncreaseWave()
    {
        _waveIndex++;
        // Add victory logic or additional conditions if necessary
        if (_waveIndex >= _levelData.AmountOfWaves)
        {
            // Victory condition or additional logic can be added here
        }
    }

    /// <summary>
    /// Spawns a new wave of enemies based on the level data and wave index.
    /// </summary>
    private void SpawnWave()
    {
        // Calculate the enemy budget based on the current wave index
        int enemyBudget = Mathf.RoundToInt(_levelData.EnemyStartingBudget * (1 + ((_levelData.EnemyBudgetMultiplier - 1) * _waveIndex)));
        BaseEnemy[] enemiesToSpawn = EnemyWaveGenerator.GenerateEnemyWave(_levelData.availableEnemyPrefabs, enemyBudget);
        if (enemiesToSpawn != null && enemiesToSpawn.Length > 0)
        {
            EventBus<SpawnEnemyEvent>.Publish(new SpawnEnemyEvent(enemiesToSpawn)); // Notify that enemies should be spawned
        }
    }
}
