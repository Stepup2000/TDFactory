using System;
using UnityEngine;

/// <summary>
/// A generic event bus that facilitates event-based communication between components.
/// Allows subscribing, unsubscribing, and publishing events of type T.
/// Implements the observer pattern to decouple event producers and consumers.
/// </summary>
/// <typeparam name="T">Type of the event that the EventBus will handle. Must be a subclass of Event.</typeparam>
public class EventBus<T> where T : Event
{
    private static event Action<T> OnEvent; // Event triggered when an event of type T is published

    /// <summary>
    /// Subscribes a handler to the event bus for this event type.
    /// </summary>
    /// <param name="handler">The action to be invoked when the event is published.</param>
    public static void Subscribe(Action<T> handler)
    {
        OnEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a handler from the event bus for this event type.
    /// </summary>
    /// <param name="handler">The action to be removed from the invocation list.</param>
    public static void UnSubscribe(Action<T> handler)
    {
        OnEvent -= handler;
    }

    /// <summary>
    /// Publishes an event to all subscribed handlers.
    /// </summary>
    /// <param name="pEvent">The event to be published.</param>
    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

// Event to initialize a new level with the provided level data.
public class InitializeLevel : Event
{
    public readonly LevelData data; // Data related to the new level

    /// <summary>
    /// Constructor for the InitializeLevel event.
    /// </summary>
    /// <param name="newData">The level data to initialize with.</param>
    public InitializeLevel(LevelData newData)
    {
        data = newData;
    }
}

// Event to signal that a new wave has started.
public class WaveStarted : Event
{
    public readonly float value; // Value associated with the new wave (e.g., wave number)

    /// <summary>
    /// Constructor for the WaveStarted event.
    /// </summary>
    /// <param name="newValue">The value to associate with the new wave.</param>
    public WaveStarted(float newValue)
    {
        value = newValue;
    }
}

// Event to request spawning a specific type of enemy.
public class SpawnEnemyEvent : Event
{
    public readonly BaseEnemy[] enemies; // Array of enemy prefabs to be spawned

    /// <summary>
    /// Constructor for the SpawnEnemyEvent event.
    /// </summary>
    /// <param name="pEnemyPrefab">The array of enemies to spawn.</param>
    public SpawnEnemyEvent(BaseEnemy[] pEnemyPrefab)
    {
        enemies = pEnemyPrefab;
    }
}

// Event to indicate that enemy spawning has been stopped.
public class StoppedSpawningEvent : Event
{
    /// <summary>
    /// Constructor for the StoppedSpawningEvent event.
    /// </summary>
    public StoppedSpawningEvent()
    {
    }
}

// Event to request a change in the current money amount.
public class ChangeMoneyEvent : Event
{
    public readonly float amount; // Amount to change the current money by

    /// <summary>
    /// Constructor for the ChangeMoneyEvent event.
    /// </summary>
    /// <param name="newAmount">The amount to change the money by.</param>
    public ChangeMoneyEvent(float newAmount)
    {
        amount = newAmount;
    }
}

// Event to notify that the total money balance has been changed.
public class TotalMoneyChangedEvent : Event
{
    public readonly float value; // New total money balance

    /// <summary>
    /// Constructor for the TotalMoneyChangedEvent event.
    /// </summary>
    /// <param name="newValue">The new total money balance.</param>
    public TotalMoneyChangedEvent(float newValue)
    {
        value = newValue;
    }
}

// Event to request a change in the current health amount.
public class ChangeHealthEvent : Event
{
    public readonly float amount; // Amount to change the current health by

    /// <summary>
    /// Constructor for the ChangeHealthEvent event.
    /// </summary>
    /// <param name="newAmount">The amount to change the health by.</param>
    public ChangeHealthEvent(float newAmount)
    {
        amount = newAmount;
    }
}

// Event to notify that the total health balance has been changed.
public class TotalHealthChangedEvent : Event
{
    public readonly float value; // New total health balance

    /// <summary>
    /// Constructor for the TotalHealthChangedEvent event.
    /// </summary>
    /// <param name="newValue">The new total health balance.</param>
    public TotalHealthChangedEvent(float newValue)
    {
        value = newValue;
    }
}

// Event to request information about active cameras.
public class RequestActiveCameras : Event
{
    public readonly Camera camera; // Camera associated with the request

    /// <summary>
    /// Constructor for the RequestActiveCameras event.
    /// </summary>
    /// <param name="newCamera">The camera related to the request.</param>
    public RequestActiveCameras(Camera newCamera)
    {
        camera = newCamera;
    }
}

// Event to toggle the visibility of weapon module buttons.
public class ToggleWeaponModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse; // Indicates whether to show or hide the buttons

    /// <summary>
    /// Constructor for the ToggleWeaponModuleButtonsEvent event.
    /// </summary>
    /// <param name="newValue">True to show buttons, false to hide them.</param>
    public ToggleWeaponModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

// Event to toggle the visibility of detection module buttons.
public class ToggleDetectionModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse; // Indicates whether to show or hide the buttons

    /// <summary>
    /// Constructor for the ToggleDetectionModuleButtonsEvent event.
    /// </summary>
    /// <param name="newValue">True to show buttons, false to hide them.</param>
    public ToggleDetectionModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

// Event to toggle the visibility of body module buttons.
public class ToggleBodyModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse; // Indicates whether to show or hide the buttons

    /// <summary>
    /// Constructor for the ToggleBodyModuleButtonsEvent event.
    /// </summary>
    /// <param name="newValue">True to show buttons, false to hide them.</param>
    public ToggleBodyModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

// Event to toggle the visibility of load buttons.
public class ToggleLoadButtonsEvent : Event
{
    public readonly bool trueOrFalse; // Indicates whether to show or hide the buttons

    /// <summary>
    /// Constructor for the ToggleLoadButtonsEvent event.
    /// </summary>
    /// <param name="newValue">True to show buttons, false to hide them.</param>
    public ToggleLoadButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

// Event to request module data. Typically used for querying or updating module-related information.
public class RequestModuleDataEvent : Event
{
    /// <summary>
    /// Constructor for the RequestModuleDataEvent event.
    /// </summary>
    public RequestModuleDataEvent()
    {
    }
}
