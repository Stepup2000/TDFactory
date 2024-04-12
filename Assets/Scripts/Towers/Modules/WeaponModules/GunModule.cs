using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunModule : MonoBehaviour, IWeapon
{
    private Tower _parentTower;
    [SerializeField] private BaseBullet _bulletPrefab;
    [field: SerializeField] public float damageMultiplier { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    [SerializeField] private Transform _bulletSpawnLocation;

    [SerializeField] private AudioClip _audioClip = null;
    private VisualEffect vfx = null;
    private ParticleSystem shellParticle = null;

    private bool _isReloading = false;
   

    private void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    private void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    private void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
    }

    private void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
        shellParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject, modulePrefab);
    }

    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }

    private void SetEnemyRequestSubscription()
    {
        if (_parentTower != null)
        {
            _parentTower.OnEnemyDetectedEvent -= AttemptFire;
            _parentTower.OnEnemyDetectedEvent += AttemptFire;
        }
        else Invoke("SetEnemyRequestSubscription", 0.5f);
    }

    //Try to fire when not reloading
    private void AttemptFire(BaseEnemy enemy, float damage)
    {
        if (!_isReloading)
        {
            CreateBullet(damage * damageMultiplier);
            _isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private void CreateBullet(float damage)
    {
        if (_bulletPrefab != null)
        {
            IBullet bullet = Instantiate(_bulletPrefab, _bulletSpawnLocation.position, transform.rotation.normalized);
            bullet.Initialize(damage);
            if (vfx != null) vfx.Play();
            if (shellParticle != null) shellParticle.Play();
            SoundManager.Instance.PlaySoundAtLocation(_audioClip, transform.position, true);
        }        
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}