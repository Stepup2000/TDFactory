using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTextUpdater : BaseTextUpdater
{
    /// <summary>
    /// Subscribes to the event when the total money changes to update the displayed text.
    /// </summary>
    public override void OnEnable()
    {
        EventBus<TotalMoneyChangedEvent>.Subscribe(ChangeMoneyText);
    }

    /// <summary>
    /// Unsubscribes from the event when the total money changes to stop updating the displayed text.
    /// </summary>
    public override void OnDisable()
    {
        EventBus<TotalMoneyChangedEvent>.UnSubscribe(ChangeMoneyText);
    }

    /// <summary>
    /// Updates the text display with the new total money value.
    /// </summary>
    /// <param name="pEvent">Event containing the updated total money value.</param>
    protected void ChangeMoneyText(TotalMoneyChangedEvent pEvent)
    {
        UpdateText(_textToDisplay + "" + pEvent.value);
    }
}
