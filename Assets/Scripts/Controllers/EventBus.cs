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
