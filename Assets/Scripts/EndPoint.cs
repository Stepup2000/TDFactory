using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            EventBus<ChangeHealthEvent>.Publish(new ChangeHealthEvent(-1));
        }
    }
}
