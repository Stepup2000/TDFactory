using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModule : MonoBehaviour, IWeapon
{
    [SerializeField] private Tower _parentTower;
    [field: SerializeField] public int damage { get; set; }
    [field: SerializeField] public float shootCooldown { get; set; }

    private bool _isReloading = false;

    private void Awake()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent += AttemptFire;
    }

    private void OnDestroy()
    {
        if (_parentTower != null) _parentTower.OnEnemyDetectedEvent -= AttemptFire;
    }

    private void AttemptFire(BaseEnemy enemy)
    {
        if (!_isReloading)
        {
            Debug.Log("fire");

            _isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(shootCooldown);
        _isReloading = false;
    }
}