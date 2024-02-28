using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule : MonoBehaviour, IWeapon
{
    [SerializeField] private Tower _parentTower;
    [SerializeField] private BaseBullet _bulletPrefab;
    [field: SerializeField] public float damageMultiplier { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }
    [field: SerializeField] public int cost { get; set; }

    private bool _isReloading = false;

    private void OnEnable()
    {
        Invoke("SetEnemyRequestSubscription", 0.5f);
    }

    private void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
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
            IBullet bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation.normalized);
            bullet.Initialize(damage);
        }        
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}