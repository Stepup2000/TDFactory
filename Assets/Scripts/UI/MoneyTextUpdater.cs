using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTextUpdater : BaseTextUpdater
{
    public override void OnEnable()
    {
        EventBus<TotalMoneyChangedEvent>.Subscribe(ChangeMoneyText);
    }

    public override void OnDisable()
    {
        EventBus<TotalMoneyChangedEvent>.UnSubscribe(ChangeMoneyText);
    }

    protected void ChangeMoneyText(TotalMoneyChangedEvent pEvent)
    {
        UpdateText(_textToDisplay + "" + pEvent.value);
    }
}
