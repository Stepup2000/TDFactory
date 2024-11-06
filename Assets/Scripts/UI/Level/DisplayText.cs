using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        SetTextComponent();
    }

    /// <summary>
    /// //Sets the text component, if no text is given try and get it from a child
    /// </summary>
    private void SetTextComponent()
    {
        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    /// <summary>
    /// Updates the text component
    /// </summary>
    public void ChangeText(string newText)
    {
        if (text != null)
            text.text = newText;            
    }
}
