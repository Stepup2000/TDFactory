using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTextUpdater : BaseTextUpdater
{
    public override void OnEnable()
    {
        EventBus<WaveStarted>.Subscribe(ChangeWaveText);
    }

    public override void OnDisable()
    {
        EventBus<WaveStarted>.UnSubscribe(ChangeWaveText);
    }

    protected void ChangeWaveText(WaveStarted pEvent)
    {
        UpdateText(_textToDisplay + "" + pEvent.value);
    }
}
