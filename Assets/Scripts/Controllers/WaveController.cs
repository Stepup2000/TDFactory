using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private LevelData _levelData;
    private enum WaveState { Start, Spawning, Fighting, End }
    private WaveState waveState = WaveState.Start;

    private int _waveIndex = 1;

    private static WaveController instance;

    //Make sure there is only one instance
    public static WaveController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WaveController>();
                if (instance == null)
                {
                    GameObject singletonObject = new();
                    instance = singletonObject.AddComponent<WaveController>();
                    singletonObject.name = new string("WaveController");
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        EventBus<StoppedSpawningEvent>.Subscribe(SetWaveToFighting);
        EventBus<InitializeLevel>.Subscribe(StartGameLoop);
    }

    private void OnDisable()
    {
        EventBus<StoppedSpawningEvent>.UnSubscribe(SetWaveToFighting);
        StopCoroutine(GameLoop());
        EventBus<InitializeLevel>.UnSubscribe(StartGameLoop);
    }

    private void StartGameLoop(InitializeLevel pEvent)
    {
        ValidateLevelData(pEvent);
        if (_levelData != null) StartCoroutine(GameLoop());
        else Debug.LogWarning("No LevelData have been assigned in the controller");
    }

    private void ValidateLevelData(InitializeLevel pEvent)
    {
        _levelData = pEvent.data;
        if (_levelData == null) Debug.LogWarning("No LevelDataLoaded");
        if (_levelData.availableEnemyPrefabs == null || _levelData.availableEnemyPrefabs.Length <= 0) Debug.LogWarning("No EnemyPrefabsLoaded");
        if (_levelData.AmountOfWaves <= 0) Debug.LogWarning("WaveAmount too little");
        if (_levelData.StartingHealth <= 0) Debug.LogWarning("StartingHealth is too low");
        if (_levelData.StartingCurrency < 100) Debug.LogWarning("StartingCurrency is below 100");
        if (_levelData.EnemyStartingBudget < 10) Debug.LogWarning("EnemyStartingBudget is below 10");
        if (_levelData.EnemyBudgetMultiplier < 1) Debug.LogWarning("EnemyBudgetMultiplier is below 1");
    }

    IEnumerator GameLoop()
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
                    if (AreAllEnemiesDefeated() == true)
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

    private void SetWaveToFighting(StoppedSpawningEvent pEvent)
    {
        if (waveState == WaveState.Spawning) waveState = WaveState.Fighting;
    }

    private bool AreAllEnemiesDefeated()
    {
        if (FindObjectOfType<BaseEnemy>()) return false;
        else return true;
    }

    private void IncreaseWave()
    {
        _waveIndex++;
        if (_waveIndex >= _levelData.AmountOfWaves)
        {
            //Victory
        }
    }

    private void SpawnWave()
    {
        int enemybudget = Mathf.RoundToInt(_levelData.EnemyStartingBudget * ( 1 + ((_levelData.EnemyBudgetMultiplier - 1) * _waveIndex)));
        BaseEnemy[] enemiesToSpawn = EnemyWaveGenerator.GenerateEnemyWave(_levelData.availableEnemyPrefabs, enemybudget);
        if (enemiesToSpawn != null && enemiesToSpawn.Length > 0) EventBus<SpawnEnemyEvent>.Publish(new SpawnEnemyEvent(enemiesToSpawn));
    }
}


