using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Represents a weapon module that fires bullets. Implements the IWeapon interface to handle shooting and reloading mechanics.
/// </summary>
public class GunModule : MonoBehaviour, IWeapon
{
    [Header("Basic gun references")]
    [SerializeField] protected BaseBullet _bulletPrefab;
    [SerializeField] protected AudioClip _audioClip = null;
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    [Header("Basic gun settings")]
    [SerializeField] protected Transform _bulletSpawnLocation;
    [field: SerializeField] public float damageMultiplier { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public float recoilAmount { get; set; }

    protected Tower _parentTower;
    protected VisualEffect vfx = null;
    protected ParticleSystem shellParticle = null;
    protected bool _isReloading = false;

    /// <summary>
    /// Called when the script is enabled. Sets up the enemy detection subscription and module data subscription.
    /// </summary>
    protected virtual void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the module data event.
    /// </summary>
    protected virtual void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is destroyed. Unsubscribes from the enemy detection event of the parent tower.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
    }

    /// <summary>
    /// Called on start. Initializes the visual effects and shell particle system.
    /// </summary>
    protected virtual void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
        shellParticle = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Handles the RequestModuleDataEvent by placing the module.
    /// </summary>
    /// <param name="requestModuleDataEvent">The event data.</param>
    protected virtual void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject, modulePrefab);
    }

    /// <summary>
    /// Sets the parent tower of this module.
    /// </summary>
    /// <param name="newTower">The new parent tower.</param>
    public virtual void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }

    /// <summary>
    /// Subscribes to the enemy detection event of the parent tower.
    /// </summary>
    protected virtual void SetEnemyRequestSubscription()
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
    protected virtual void AttemptFire(BaseEnemy enemy, float damage)
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
    protected virtual void CreateBullet(float damage)
    {
        if (_bulletPrefab != null)
        {
            // Generate small random horizontal recoil (Y-axis only)
            float recoilY = Random.Range(-recoilAmount, recoilAmount);
            Quaternion recoilRotation = Quaternion.Euler(0f, recoilY, 0f) * transform.rotation;

            // Instantiate bullet with recoil
            IBullet bullet = Instantiate(_bulletPrefab, _bulletSpawnLocation.position, recoilRotation);
            bullet.Initialize(damage);

            if (vfx != null) vfx.Play();
            if (shellParticle != null) shellParticle.Play();
            SoundManager.Instance.PlaySoundAtLocation(_audioClip, transform.position);
        }
    }


    /// <summary>
    /// Coroutine for handling the reload time of the module.
    /// </summary>
    protected virtual IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}
