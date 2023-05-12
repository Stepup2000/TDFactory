using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour, IBullet
{
    [field: SerializeField] public float speed { get; set; }
    [field: SerializeField] public float damage { get; set; }
    public void Initialize(float pDamage) 
    {
        damage = pDamage;
    }

    private void Move()
    {
        // Get the forward direction of the object
        Vector3 direction = transform.forward;

        // Move the object in the forward direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void Update()
    {
        Move();
    }
}
