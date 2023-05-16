using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float maxHealth { get; set; }
    void TakeDamage(float damage) { }
    bool IsStillAlive();
    void Death();
}
