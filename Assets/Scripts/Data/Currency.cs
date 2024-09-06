using UnityEngine;

/// <summary>
/// ScriptableObject for managing currency data in the game.
/// </summary>
[CreateAssetMenu(fileName = "New Currency", menuName = "CurrencyData")]
public class Currency : ScriptableObject
{
    [SerializeField]
    private float _currentAmount;

    /// <summary>
    /// Called when the ScriptableObject is enabled. Subscribes to the TotalMoneyChangedEvent to update the currency amount.
    /// </summary>
    private void OnEnable()
    {
        EventBus<TotalMoneyChangedEvent>.Subscribe(ChangeCurrency);
    }

    /// <summary>
    /// Called when the ScriptableObject is disabled. Unsubscribes from the TotalMoneyChangedEvent.
    /// </summary>
    private void OnDisable()
    {
        EventBus<TotalMoneyChangedEvent>.UnSubscribe(ChangeCurrency);
    }

    /// <summary>
    /// Updates the current currency amount based on the TotalMoneyChangedEvent.
    /// </summary>
    /// <param name="pEvent">Event containing the new currency value.</param>
    public void ChangeCurrency(TotalMoneyChangedEvent pEvent)
    {
        _currentAmount = pEvent.value;
    }

    /// <summary>
    /// Returns the current amount of currency.
    /// </summary>
    /// <returns>The current amount of currency.</returns>
    public float GetCurrentAmount()
    {
        return _currentAmount;
    }
}
