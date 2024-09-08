using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTextUpdater : BaseTextUpdater
{
    /// <summary>
    /// Subscribes to the event when the total health changes to update the displayed text.
    /// </summary>
    public override void OnEnable()
    {
        EventBus<TotalHealthChangedEvent>.Subscribe(ChangeHealthText);
    }

    /// <summary>
    /// Unsubscribes from the event when the total health changes to stop updating the displayed text.
    /// </summary>
    public override void OnDisable()
    {
        EventBus<TotalHealthChangedEvent>.UnSubscribe(ChangeHealthText);
    }

    /// <summary>
    /// Updates the text display with the new total health value.
    /// </summary>
    /// <param name="pEvent">Event containing the updated total health value.</param>
    protected void ChangeHealthText(TotalHealthChangedEvent pEvent)
    {
        UpdateText(_textToDisplay + "" + pEvent.value);
    }
}
