using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the wave system for the game, including spawning enemies and handling wave transitions.
/// Implements a singleton pattern to ensure only one instance of the controller exists.
/// </summary>
public class WaveController : MonoBehaviour
{
    private LevelData _levelData;
    private enum WaveState { Start, Spawning, Fighting, End }
    private WaveState waveState = WaveState.Start;

    private int _waveIndex = 1;
    private static WaveController instance;

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
        StopCoroutine(GameLoop());
        EventBus<InitializeLevel>.UnSubscribe(StartGameLoop);
    }

    /// <summary>
    /// Starts the game loop coroutine and validates level data.
    /// </summary>
    /// <param name="pEvent">Event containing the level data.</param>
    private void StartGameLoop(InitializeLevel pEvent)
    {
        ValidateLevelData(pEvent);
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
                    SpawnWave();
                    waveState = WaveState.Spawning;
                    EventBus<WaveStarted>.Publish(new WaveStarted(_waveIndex));
                    break;

                case WaveState.Spawning:
                    yield return new WaitForSeconds(1f);
                    break;

                case WaveState.Fighting:
                    if (AreAllEnemiesDefeated())
                    {
                        waveState = WaveState.End;
                    }
                    else yield return new WaitForSeconds(1f);
                    break;

                case WaveState.End:
                    IncreaseWave();
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
        return FindObjectOfType<BaseEnemy>() == null;
    }

    /// <summary>
    /// Gets the index of the current wave.
    /// </summary>
    public int GetWaveIndex()
    {
        if (_waveIndex > 0) return _waveIndex;
        else return 1;
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
        int enemyBudget = Mathf.RoundToInt(_levelData.EnemyStartingBudget * (1 + ((_levelData.EnemyBudgetMultiplier - 1) * _waveIndex)));
        BaseEnemy[] enemiesToSpawn = EnemyWaveGenerator.GenerateEnemyWave(_levelData.availableEnemyPrefabs, enemyBudget);
        if (enemiesToSpawn != null && enemiesToSpawn.Length > 0)
        {
            EventBus<SpawnEnemyEvent>.Publish(new SpawnEnemyEvent(enemiesToSpawn));
        }
    }
}
