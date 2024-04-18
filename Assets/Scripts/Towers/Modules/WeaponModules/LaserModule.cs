using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LaserModule : MonoBehaviour, IWeapon
{
    private Tower _parentTower;
    [SerializeField] private BaseBullet _bulletPrefab;
    [field: SerializeField] public float damageMultiplier { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    [SerializeField] private GameObject _Laser;

    [SerializeField] private Transform _bulletSpawnLocation;

    [SerializeField] private AudioClip _audioClip = null;
    private ParticleSystem shellParticle = null;

    private Coroutine _laserTimerCoroutine;
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

    // Coroutine for the laser timer
    private IEnumerator LaserTimer()
    {
        if (_Laser.gameObject.activeSelf == false) _Laser.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        // Deactivate the laser after the timer expires
        _Laser.gameObject.SetActive(false);
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}