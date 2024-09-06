using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to an endpoint.
/// It handles the detection of enemies reaching the endpoint and triggers a health change event.
/// </summary>
public class EndPoint : MonoBehaviour
{
    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// If the collider belongs to a BaseEnemy, publishes a health change event to indicate damage.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a BaseEnemy
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            // Publish a health change event with a negative value indicating damage
            EventBus<ChangeHealthEvent>.Publish(new ChangeHealthEvent(-1));
        }
    }
}
