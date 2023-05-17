using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private Currency _currency;
    private static MoneyController instance;
    private float _currentMoney;

    //Make sure there is only one instance
    public static MoneyController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MoneyController>();
                if (instance == null)
                {
                    GameObject singletonObject = new();
                    instance = singletonObject.AddComponent<MoneyController>();
                    singletonObject.name = new string("MoneyController");
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        EventBus<ChangeMoneyEvent>.Subscribe(ChangeMoney);
    }

    private void OnDisable()
    {
        EventBus<ChangeMoneyEvent>.UnSubscribe(ChangeMoney);
    }

    private void ChangeMoney(ChangeMoneyEvent pEvent)
    {
        _currentMoney += pEvent.amount;
        if (_currentMoney < 0) _currentMoney = 0;
        EventBus<TotalMoneyChangedEvent>.Publish(new TotalMoneyChangedEvent(_currentMoney));
    }

    public bool CanAfford(float cost)
    {
        return cost <= _currentMoney;
    }
}
