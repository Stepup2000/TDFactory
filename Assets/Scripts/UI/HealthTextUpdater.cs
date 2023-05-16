using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTextUpdater : BaseTextUpdater
{
    public override void OnEnable()
    {
        EventBus<TotalHealthChangedEvent>.Subscribe(ChangeHealthText);
    }

    public override void OnDisable()
    {
        EventBus<TotalHealthChangedEvent>.UnSubscribe(ChangeHealthText);
    }

    protected void ChangeHealthText(TotalHealthChangedEvent pEvent)
    {
        UpdateText(_textToDisplay + "" + pEvent.value);
    }
}
