using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(LocalizeStringEvent))]
public class ChangeLocalizedString : MonoBehaviour
{
    protected TMP_Text text;
    protected LocalizeStringEvent localizedStringEvent;
    protected LocalizedString localizedString;

    protected virtual void OnEnable()
    {
        text = GetComponent<TMP_Text>();
        localizedStringEvent = GetComponent<LocalizeStringEvent>();
        localizedString = new LocalizedString();
    }

    protected virtual void OnDisable()
    {
        localizedString.StringChanged -= UpdateText;
    }

    /// <summary>
    /// Updates the LocalizedString to hold a new string and optional arguments.
    /// Supports both regular and smart strings.
    /// </summary>
    public virtual void ChangeString(string tableName, string key, float newAmount = -1, params object[] args)
    {
        if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(key)) return;

        LocalizedString newLocalizedString = new LocalizedString();

        newLocalizedString.TableReference = tableName;
        newLocalizedString.TableEntryReference = key;

        List<object> allArgs = new List<object>();
        if (newAmount != -1) allArgs.Add(newAmount);
        allArgs.AddRange(args);

        newLocalizedString.Arguments = allArgs.ToArray();

        localizedStringEvent.StringReference = newLocalizedString;

        newLocalizedString.StringChanged -= UpdateText;
        newLocalizedString.StringChanged += UpdateText;
    }

    /// <summary>
    /// Updates the actual text.
    /// </summary>
    protected virtual void UpdateText(string value)
    {
        text.text = value;
    }
}
