using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule : MonoBehaviour, IWeapon
{
    [SerializeField] private Tower _parentTower;
    [SerializeField] private BaseBullet _bulletPrefab;
    [field: SerializeField] public float damageMultiplier { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }

    private bool _isReloading = false;

    private void OnEnable()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent += AttemptFire;
    }

    private void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
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