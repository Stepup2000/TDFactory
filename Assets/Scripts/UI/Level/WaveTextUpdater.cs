using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTextUpdater : BaseTextUpdater
{
    /// <summary>
    /// Subscribes to the WaveStarted event when the object is enabled.
    /// </summary>
    public override void OnEnable()
    {
        EventBus<WaveStarted>.Subscribe(ChangeWaveText);
    }

    /// <summary>
    /// Unsubscribes from the WaveStarted event when the object is disabled.
    /// </summary>
    public override void OnDisable()
    {
        EventBus<WaveStarted>.UnSubscribe(ChangeWaveText);
    }

    /// <summary>
    /// Updates the displayed text with the current wave value.
    /// </summary>
    /// <param name="pEvent">The event containing the wave value.</param>
    protected void ChangeWaveText(WaveStarted pEvent)
    {
        UpdateText(_textToDisplay + " " + pEvent.value);
    }
}
