using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Represents a weapon module that fires bullets. Implements the IWeapon interface to handle shooting and reloading mechanics.
/// </summary>
public class GunModule : MonoBehaviour, IWeapon
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
    /// The location where bullets are spawned.
    /// </summary>
    [SerializeField] private Transform _bulletSpawnLocation;

    /// <summary>
    /// The sound clip played when firing the gun.
    /// </summary>
    [SerializeField] private AudioClip _audioClip = null;

    /// <summary>
    /// The visual effect played when firing.
    /// </summary>
    private VisualEffect vfx = null;

    /// <summary>
    /// The particle system for the shell effect.
    /// </summary>
    private ParticleSystem shellParticle = null;

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
    /// Called on start. Initializes the visual effects and shell particle system.
    /// </summary>
    private void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
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
    /// Attempts to fire a bullet at the detected enemy if not reloading.
    /// </summary>
    /// <param name="enemy">The enemy detected.</param>
    /// <param name="damage">The damage to apply.</param>
    private void AttemptFire(BaseEnemy enemy, float damage)
    {
        if (!_isReloading)
        {
            CreateBullet(damage * damageMultiplier);
            _isReloading = true;
            StartCoroutine(Reload());
        }
    }

    /// <summary>
    /// Creates and fires a bullet, playing the associated visual effects and sound.
    /// </summary>
    /// <param name="damage">The damage to apply to the bullet.</param>
    private void CreateBullet(float damage)
    {
        if (_bulletPrefab != null)
        {
            IBullet bullet = Instantiate(_bulletPrefab, _bulletSpawnLocation.position, transform.rotation.normalized);
            bullet.Initialize(damage);
            if (vfx != null) vfx.Play();
            if (shellParticle != null) shellParticle.Play();
            SoundManager.Instance.PlaySoundAtLocation(_audioClip, transform.position);
        }        
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
