using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Represents a weapon module that fires lasers. Implements the IWeapon interface to handle shooting and reloading mechanics.
/// </summary>
public class LaserModule : MonoBehaviour, IWeapon
{
    /// <summary>
    /// Reference to the parent tower of this module.
    /// </summary>
    private Tower _parentTower;

    /// <summary>
    /// The bullet prefab used by this module.
    /// </summary>
    [SerializeField] private BaseBullet _bulletPrefab;

    /// <summary>
    /// The damage multiplier for the weapon.
    /// </summary>
    [field: SerializeField] public float damageMultiplier { get; set; }

    /// <summary>
    /// The cooldown time between shots.
    /// </summary>
    [field: SerializeField] public float shootCooldown { get; set; }

    /// <summary>
    /// The cost of the module.
    /// </summary>
    [field: SerializeField] public int cost { get; set; }

    /// <summary>
    /// The prefab associated with this module.
    /// </summary>
    [field: SerializeField] public GameObject modulePrefab { get; set; }

    /// <summary>
    /// The sound clip played when the module is placed.
    /// </summary>
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    /// <summary>
    /// The game object representing the laser.
    /// </summary>
    [SerializeField] private GameObject _Laser;

    /// <summary>
    /// The location where bullets are spawned.
    /// </summary>
    [SerializeField] private Transform _bulletSpawnLocation;

    /// <summary>
    /// The particle system for the shell effect.
    /// </summary>
    private ParticleSystem shellParticle = null;

    /// <summary>
    /// Coroutine for timing the laser activation.
    /// </summary>
    private Coroutine _laserTimerCoroutine;

    /// <summary>
    /// Indicates if the module is currently reloading.
    /// </summary>
    private bool _isReloading = false;

    /// <summary>
    /// Called when the script is enabled. Sets up the enemy detection subscription and module data subscription.
    /// </summary>
    private void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the module data event.
    /// </summary>
    private void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is destroyed. Unsubscribes from the enemy detection event of the parent tower.
    /// </summary>
    private void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
    }

    /// <summary>
    /// Called on start. Initializes the shell particle system.
    /// </summary>
    private void Start()
    {
        shellParticle = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Handles the RequestModuleDataEvent by placing the module.
    /// </summary>
    /// <param name="requestModuleDataEvent">The event data.</param>
    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject, modulePrefab);
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
    /// Subscribes to the enemy detection event of the parent tower.
    /// </summary>
    private void SetEnemyRequestSubscription()
    {
        if (_parentTower != null)
        {
            _parentTower.OnEnemyDetectedEvent -= AttemptFire;
            _parentTower.OnEnemyDetectedEvent += AttemptFire;
        }
        else Invoke("SetEnemyRequestSubscription", 0.5f);
    }

    /// <summary>
    /// Attempts to fire the laser at the detected enemy.
    /// </summary>
    /// <param name="enemy">The enemy detected.</param>
    /// <param name="damage">The damage to apply.</param>
    private void AttemptFire(BaseEnemy enemy, float damage)
    {
        if (!_isReloading)
        {
            _isReloading = true;
            StartCoroutine(Reload());

            // Start or reset the timer coroutine
            if (_laserTimerCoroutine != null)
            {
                StopCoroutine(_laserTimerCoroutine);
            }
            _laserTimerCoroutine = StartCoroutine(LaserTimer());

            if (shellParticle != null) shellParticle.Play();
        }
    }

    /// <summary>
    /// Coroutine for the laser timer. Activates the laser and then deactivates it after a short duration.
    /// </summary>
    private IEnumerator LaserTimer()
    {
        if (_Laser.gameObject.activeSelf == false) _Laser.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        // Deactivate the laser after the timer expires
        _Laser.gameObject.SetActive(false);
    }

    /// <summary>
    /// Coroutine for handling the reload time of the module.
    /// </summary>
    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}
