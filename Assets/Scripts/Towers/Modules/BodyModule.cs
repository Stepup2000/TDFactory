using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a body module for a tower. Implements the IModule interface to handle module data and interactions with the tower.
/// </summary>
public class BodyModule : MonoBehaviour, IModule
{
    /// <summary>
    /// Reference to the parent tower of this module.
    /// </summary>
    private Tower _parentTower;

    /// <summary>
    /// The cost of the module.
    /// </summary>
    [field: SerializeField] public int cost { get; set; }

    /// <summary>
    /// The prefab associated with this module.
    /// </summary>
    [field: SerializeField] public GameObject modulePrefab { get; set; }

    /// <summary>
    /// The sound clip played when the module is placed.
    /// </summary>
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    /// <summary>
    /// Called when the script is enabled. Subscribes to the RequestModuleDataEvent to handle module data requests.
    /// </summary>
    private void OnEnable()
    {
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the RequestModuleDataEvent to stop handling module data requests.
    /// </summary>
    private void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    /// <summary>
    /// Handles the RequestModuleDataEvent by placing the module.
    /// </summary>
    /// <param name="requestModuleDataEvent">The event data.</param>
    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject, modulePrefab);
    }

    /// <summary>
    /// Sets the parent tower of this module.
    /// </summary>
    /// <param name="newTower">The new parent tower.</param>
    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }
}
