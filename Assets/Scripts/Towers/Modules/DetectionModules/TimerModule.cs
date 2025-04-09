using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerModule : MonoBehaviour, IDetectionModule
{
    [Header("Basic detection references")]
    protected Tower _parentTower;

    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private int timerDuration = 5;
    private Coroutine _fireCoroutine;

    /// <summary>
    /// Called when the script is enabled. Subscribes to the EnemyRequestEvent and RequestModuleDataEvent.
    /// Starts the firing coroutine.
    /// </summary>
    protected virtual void OnEnable()
    {
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
        _fireCoroutine = StartCoroutine(FireRoutine());
    }

    /// <summary>
    /// Sets the parent tower of this module.
    /// </summary>
    /// <param name="newTower">The new parent tower.</param>
    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the EnemyRequestEvent and RequestModuleDataEvent.
    /// Stops the firing coroutine.
    /// </summary>
    protected virtual void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that fires every 10 seconds.
    /// </summary>
    private IEnumerator FireRoutine()
    {
        while (true)
        {
            int currentTime = timerDuration;
            while (currentTime >= 0)
            {
                UpdateTimerDisplay(currentTime);
                yield return new WaitForSeconds(1f);
                currentTime--;
            }

            UpdateTimerDisplay(0);
            EnemyDetected();
        }
    }

    /// <summary>
    /// Updates the text to display the current timer.
    /// </summary>
    /// <param name="seconds">The amound of seconds to be displayed</param>
    private void UpdateTimerDisplay(int seconds)
    {
        int minutes = seconds / 60;
        int secs = seconds % 60;
        if (timerText != null)
            timerText.text = $"{minutes:00}:{secs:00}";
    }

    /// <summary>
    /// Handles the RequestModuleDataEvent by placing the module.
    /// </summary>
    /// <param name="requestModuleDataEvent">The event data.</param>
    protected virtual void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject.transform.parent.gameObject, modulePrefab);
    }

    /// <summary>
    /// Fires the weapons of thet ower by overriding the firing sequence.
    /// </summary>
    protected void EnemyDetected()
    {
        _parentTower?.TryFireWeapons(true);
    }

    /// <summary>
    /// Adds an enemy to the set of enemies in range.
    /// </summary>
    /// <param name="enemy">The detected enemy.</param>
    public void EnemyDetected(BaseEnemy enemy)
    {
        //_enemiesInRange.Add(enemy);
    }

    /// <summary>
    /// Removes an enemy from the set of enemies in range.
    /// </summary>
    /// <param name="enemy">The lost enemy.</param>
    public void EnemyLost(BaseEnemy enemy)
    {
        //_enemiesInRange.Remove(enemy);
    }
}
