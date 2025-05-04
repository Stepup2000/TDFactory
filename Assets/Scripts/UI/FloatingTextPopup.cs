using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FloatingTextPopup : MonoBehaviour, IPoolable
{
    [SerializeField, Tooltip("Direction in which the text popup will move.")]
    private Vector3 moveDirection = Vector3.up;

    [SerializeField, Tooltip("Speed at which the text popup will move.")]
    private float moveSpeed = 1f;

    [SerializeField, Tooltip("The duration it takes for the popup to fully fade out.")]
    private float fadeDuration = 1f;

    [SerializeField, Tooltip("The alpha the popup will reachwith the fadeout.")]
    private float minimumFadeAlpha = 0f;

    private TMP_Text text;

    private Coroutine moveCoroutine;

    protected void OnEnable()
    {
        TryGetComponent<TMP_Text>(out text);
    }

    /// <summary>
    /// This method will be called when the object is taken from the pool
    /// </summary>
    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// This method will be called when the object is returned to the pool
    /// </summary>
    public void OnDespawn()
    {
        StopMoving();
        gameObject.SetActive(false);
    }

    /// <summary>
    // This method is used to reset the object's internal state before reuse
    /// </summary>
    public void ResetObject()
    {
        text.text = string.Empty;
        text.color = Color.white;
        fadeDuration = 1f;
        moveDirection = Vector3.up;
        minimumFadeAlpha = 0;
    }

    /// <summary>
    /// Starts the movement of the text popup.
    /// </summary>
    public void StartMoving()
    {
        if (moveCoroutine != null) return;
        moveCoroutine = StartCoroutine(MoveAndFadeOut());
    }

    /// <summary>
    /// Stops the movement of the text popup.
    /// </summary>
    public void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    /// <summary>
    /// Configures the popup with the specified message, position, and color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="position">The world position to place the popup.</param>
    /// <param name="newSpeed">The speed the popup moves with.</param>
    /// <param name="newColor">The color of the text.</param>
    /// <param name="newFadeDuration">The fade duration of the text.</param>
    /// <param name="newDirection">The direction the pop goes to.</param>
    /// <param name="newMinimumFadeAlpha">The alpha the popup will reachwith the fadeout.</param>
    public void SetupPopup(string message, Vector3 position, float newSpeed,Color newColor, float newFadeDuration, Vector3 newDirection, float newMinimumFadeAlpha)
    {
        text.text = message;
        transform.position = position;
        moveSpeed = newSpeed;
        text.color = newColor;
        fadeDuration = newFadeDuration;
        moveDirection = newDirection;
        minimumFadeAlpha = newMinimumFadeAlpha;
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
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

            // Gradually fade out the text
            float alpha = Mathf.Lerp(1f, minimumFadeAlpha, elapsedTime / fadeDuration);
            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StopMoving();
    }
}
