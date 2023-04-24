using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public int damage { get; set; }
    public float shootCooldown { get; set; }
    public void TryShoot();
}