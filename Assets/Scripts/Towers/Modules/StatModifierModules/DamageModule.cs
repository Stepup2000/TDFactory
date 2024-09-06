using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a module that modifies the damage of a tower.
/// Implements the IStatModifier interface to apply damage modifications.
/// </summary>
public class DamageModule : MonoBehaviour, IStatModifier
{
    /// <summary>
    /// Reference to the tower that this module affects.
    /// </summary>
    [SerializeField] private Tower _parentTower;

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
    /// The damage modifier applied by this module.
    /// </summary>
    private float _damageModifier = 1f;

    /// <summary>
    /// Gets or sets the parent tower of this module.
    /// </summary>
    public Tower parentTower
    {
        get { return _parentTower; }
        set { _parentTower = value; }
    }

    /// <summary>
    /// Called when the script is enabled. Subscribes to the RequestModuleDataEvent.
    /// </summary>
    private void OnEnable()
    {
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the RequestModuleDataEvent.
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

    /// <summary>
    /// Called on start. Executes the module to apply damage modifications.
    /// </summary>
    private void Start()
    {
        ExecuteModule();
    }

    /// <summary>
    /// Applies the damage modifier to the parent tower.
    /// </summary>
    public void ExecuteModule()
    {
        Dictionary<string, float> modifier = new();
        modifier[Tower.DAMAGE_STAT] = _damageModifier;
        _parentTower.ModifyStats(modifier);
    }
}
