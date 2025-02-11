using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTextUpdater : ChangeLocalizedString
{
    /// <summary>
    /// Subscribes to the WaveStarted event when the object is enabled.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus<WaveStarted>.Subscribe(UpdateText);
    }

    /// <summary>
    /// Unsubscribes from the WaveStarted event when the object is disabled.
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus<WaveStarted>.UnSubscribe(UpdateText);
    }

    /// <summary>
    /// Updates the localized text event with the new string key.
    /// </summary>
    /// <param name="pEvent">Requires an WaveStartedEvent</param>
    protected void UpdateText(WaveStarted pEvent)
    {
        ChangeString("UI", "Wave", pEvent.value);
    }
}
