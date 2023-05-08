using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon : IModule
{
    public int damage { get; set; }
    public float shootCooldown { get; set; }
    public new void ExecuteModule();
}