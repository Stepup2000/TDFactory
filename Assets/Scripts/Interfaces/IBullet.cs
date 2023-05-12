using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    public float damage { get; set; }
    public float speed { get; set; }
    public void Initialize(float pDamage) { }
}
