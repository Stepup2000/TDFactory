using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BaseBullet
{
    private List<IDamageable> _recentlyHitEnemies = new List<IDamageable>();

    public override void HandleTriggerCollision(Collider other)
    {
        if (other.TryGetComponent(out IDamageable foundDamageAble))
        {
            if (!_recentlyHitEnemies.Contains(foundDamageAble))
            {
                if (foundDamageAble.IsStillAlive() == true)
                {
                    foundDamageAble.TakeDamage(damage);
                    _recentlyHitEnemies.Add(foundDamageAble);
                    //StartCoroutine(RemoveFromHitList(foundDamageAble, _hitDelay));
                }
            }
        }
    }

    public override void Move()
    {

    }

    /*
    private IEnumerator RemoveFromHitList(IDamageable damageable, float delay)
    {
        yield return new WaitForSeconds(delay);
        _recentlyHitEnemies.Remove(damageable);
    }
    */
}
