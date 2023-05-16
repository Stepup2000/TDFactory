using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private Currency _currency;
    private static HealthController instance;
    private float _currentHealth;

    //Make sure there is only one instance
    public static HealthController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HealthController>();
                if (instance == null)
                {
                    GameObject singletonObject = new();
                    instance = singletonObject.AddComponent<HealthController>();
                    singletonObject.name = new string("HealthController");
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        EventBus<ChangeHealthEvent>.Subscribe(ChangeHealth);
    }

    private void OnDisable()
    {
        EventBus<ChangeHealthEvent>.UnSubscribe(ChangeHealth);
    }

    private void ChangeHealth(ChangeHealthEvent pEvent)
    {
        _currentHealth += pEvent.amount;
        if (_currentHealth < 0) _currentHealth = 0;
        EventBus<TotalHealthChangedEvent>.Publish(new TotalHealthChangedEvent(_currentHealth));
    }

    public bool IsGameOver()
    {
        return _currentHealth <= 0;
    }
}
