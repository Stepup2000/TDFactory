using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon : IModule
{
    public float damageMultiplier { get; set; }
    public float shootCooldown { get; set; }
}