using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class BaseTextUpdater : MonoBehaviour
{
    [SerializeField] protected string _textToDisplay;  // The base text to be displayed before the dynamic value.
    private TextMeshProUGUI _text;  // Reference to the TextMeshProUGUI component for displaying text.

    /// <summary>
    /// Called when the component is enabled. Subscribes to the relevant event for updating the text.
    /// </summary>
    virtual public void OnEnable()
    {
        // Subscribe here to the appropriate event.
    }

    /// <summary>
    /// Called when the component is disabled. Unsubscribes from the relevant event to stop updating the text.
    /// </summary>
    virtual public void OnDisable()
    {
        // Unsubscribe here from the appropriate event.
    }

    /// <summary>
    /// Called when the script instance is being loaded. Initializes the TextMeshProUGUI component.
    /// </summary>
    public virtual void Start()
    {
        TryGetComponent<TextMeshProUGUI>(out _text);
    }

    /// <summary>
    /// Updates the displayed text with the new value.
    /// </summary>
    /// <param name="newText">The new text to display, including any dynamic values.</param>
    protected void UpdateText(string newText)
    {
        if (_text != null)
        {
            _text.text = newText;
        }
    }
}
