using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's money, ensuring there is only one instance and updating the money balance.
/// Implements a singleton pattern to ensure only one instance exists and persists across scenes.
/// </summary>
public class MoneyController : MonoBehaviour
{
    [SerializeField] private Currency _currency; // Reference to the Currency asset or component

    private static MoneyController _instance; // Singleton instance of MoneyController

    private float _currentMoney; // The current amount of money the player has

    /// <summary>
    /// Provides access to the singleton instance of MoneyController.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    public static MoneyController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MoneyController>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MoneyController).Name);
                    _instance = singletonObject.AddComponent<MoneyController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Subscribes to the ChangeMoneyEvent when enabled.
    /// </summary>
    private void OnEnable()
    {
        EventBus<ChangeMoneyEvent>.Subscribe(ChangeMoney);
    }

    /// <summary>
    /// Unsubscribes from the ChangeMoneyEvent when disabled.
    /// </summary>
    private void OnDisable()
    {
        EventBus<ChangeMoneyEvent>.UnSubscribe(ChangeMoney);
    }

    /// <summary>
    /// Updates the current money based on the ChangeMoneyEvent.
    /// Publishes a TotalMoneyChangedEvent to notify other systems of the updated money balance.
    /// </summary>
    /// <param name="pEvent">The event containing the amount to change the money by.</param>
    private void ChangeMoney(ChangeMoneyEvent pEvent)
    {
        _currentMoney += pEvent.amount;
        if (_currentMoney < 0) _currentMoney = 0;
        EventBus<TotalMoneyChangedEvent>.Publish(new TotalMoneyChangedEvent(_currentMoney));

        Color newcolor;
        if (pEvent.amount >= 0) newcolor = Color.green;
        else newcolor = Color.red;

        FloatingTextController.Instance.ShowTextPopup("$" + pEvent.amount, pEvent.position, newcolor, 1f);
    }

    /// <summary>
    /// Checks if the player can afford an item with the specified cost.
    /// </summary>
    /// <param name="cost">The cost of the item.</param>
    /// <returns>True if the player has enough money, otherwise false.</returns>
    public bool CanAfford(float cost)
    {
        return cost <= _currentMoney;
    }
}
