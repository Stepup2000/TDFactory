using System;
using UnityEngine;
using System.Collections.Generic;

public class EventBus<T> where T : Event
{
    public static event Action<T> OnEvent;

    public static void Subscribe(Action<T> handler)
    {
        OnEvent += handler;
    }

    public static void UnSubscribe(Action<T> handler)
    {
        OnEvent -= handler;
    }

    public static void Publish(T pEvent)
    {
        OnEvent?.Invoke(pEvent);
    }
}

//Sends out an event with all the information for the new level
public class InitializeLevel : Event
{
    public readonly LevelData data;
    public InitializeLevel(LevelData newData)
    {
        data = newData;
    }
}

//Sends out an event to let everything know a new wave has started
public class WaveStarted : Event
{
    public readonly float value;
    public WaveStarted(float newValue)
    {
        value = newValue;
    }
}

//Sends out an event to spawn a specific type of enemy
public class SpawnEnemyEvent : Event
{
    public readonly BaseEnemy[] enemies;
    public SpawnEnemyEvent(BaseEnemy[] pEnemyPrefab)
    {
        enemies = pEnemyPrefab;
    }
}

//Sends out an event that the EnemySpawners are done spawning
public class StoppedSpawningEvent : Event
{
    public StoppedSpawningEvent()
    {
    }
}

//Sends out an event to change the current money
public class ChangeMoneyEvent : Event
{
    public readonly float amount;
    public ChangeMoneyEvent(float newAmount)
    {
        amount = newAmount;
    }
}

//Sends out an event to let everything know the current money balance has been changed
public class TotalMoneyChangedEvent : Event
{
    public readonly float value;
    public TotalMoneyChangedEvent(float newValue)
    {
        value = newValue;
    }
}

//Sends out an event to change the current health
public class ChangeHealthEvent : Event
{
    public readonly float amount;
    public ChangeHealthEvent(float newAmount)
    {
        amount = newAmount;
    }
}

//Sends out an event to let everything know the current health balance has been changed
public class TotalHealthChangedEvent : Event
{
    public readonly float value;
    public TotalHealthChangedEvent(float newValue)
    {
        value = newValue;
    }
}
public class ToggleWeaponModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse;
    public ToggleWeaponModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

public class ToggleDetectionModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse;
    public ToggleDetectionModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}
public class ToggleBodyModuleButtonsEvent : Event
{
    public readonly bool trueOrFalse;
    public ToggleBodyModuleButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

public class ToggleLoadButtonsEvent : Event
{
    public readonly bool trueOrFalse;
    public ToggleLoadButtonsEvent(bool newValue)
    {
        trueOrFalse = newValue;
    }
}

public class RequestModuleDataEvent : Event
{
    public RequestModuleDataEvent()
    {
    }
}