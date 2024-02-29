using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class BaseTextUpdater : MonoBehaviour
{
    [SerializeField] protected string _textToDisplay;
    private TextMeshProUGUI _text;

    virtual public void OnEnable()
    {
        //Subscribe here to the right event
    }

    virtual public void OnDisable()
    {
        //UnSubscribe here to the right event
    }

    public virtual void Start()
    {
        TryGetComponent<TextMeshProUGUI>(out _text);
    }

    protected void UpdateText(string newText)
    {
        if (_text != null) _text.text = newText;
    }
}
