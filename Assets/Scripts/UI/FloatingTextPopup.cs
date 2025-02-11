using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FloatingTextPopup : MonoBehaviour
{
    [SerializeField, Tooltip("Direction in which the text popup will move.")]
    private Vector3 moveDirection = Vector3.up;

    [SerializeField, Tooltip("Speed at which the text popup will move.")]
    private float moveSpeed = 1f;

    private TMP_Text text;    
    private float fadeDuration = 1f;

    /// <summary>
    /// Starts the movement of the text popup.
    /// </summary>
    public void StartMoving()
    {
        moveDirection = Vector3.up;
        StartCoroutine(MoveAndFadeOut());
    }

    /// <summary>
    /// Stops the movement of the text popup.
    /// </summary>
    public void StopMoving()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Configures the popup with the specified message, position, and color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="position">The world position to place the popup.</param>
    /// <param name="newColor">The color of the text.</param>
    public void SetupPopup(string message, Vector3 position, Color newColor, float newFadeDuration)
    {
        if(!TryGetComponent<TMP_Text>(out text)) return;
        transform.position = position;
        text.text = message;
        text.color = newColor;
        fadeDuration = newFadeDuration;
        StartMoving();
    }

    /// <summary>
    /// Coroutine to handle movement and fading out of the text popup over time.
    /// </summary>
    private IEnumerator MoveAndFadeOut()
    {
        float elapsedTime = 0f;
        Color initialColor = text.color;

        while (elapsedTime < fadeDuration)
        {
            // Move upwards over time
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

            // Gradually fade out the text
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
