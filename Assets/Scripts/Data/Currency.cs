using UnityEngine;

[CreateAssetMenu(fileName = "New Currency", menuName = "CurrencyData")]
public class Currency : ScriptableObject
{
    [SerializeField] private float _currentAmount;

    private void OnEnable()
    {
        EventBus<TotalMoneyChangedEvent>.Subscribe(ChangeCurrency);    
    }

    private void OnDisable()
    {
        EventBus<TotalMoneyChangedEvent>.UnSubscribe(ChangeCurrency);
    }

    public void ChangeCurrency(TotalMoneyChangedEvent pEvent)
    {
        _currentAmount = pEvent.value;
    }
}