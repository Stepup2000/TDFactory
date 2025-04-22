using System.Collections;
using UnityEngine;

public class FloatingTextController : BaseObjectPooler<FloatingTextPopup>
{
    // Singleton instance
    protected static FloatingTextController _instance;

    /// <summary>
    /// Gets the singleton instance of the FloatingTextController. Creates one if it does not exist.
    /// </summary>
    public static FloatingTextController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FloatingTextController>();
                if (_instance == null)
                {
                    // Create a new GameObject with FloatingTextController component if none exists
                    GameObject controllerObject = new GameObject("FloatingTextController");
                    _instance = controllerObject.AddComponent<FloatingTextController>();
                }
            }
            return _instance;
        }
    }

    [SerializeField, Tooltip("Prefab for the floating text popup.")]
    protected FloatingTextPopup textPopupPrefab;

    protected override void Awake()
    {
        // Ensure this instance is the only one, destroy duplicates
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate FloatingTextController instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // Set the prefab for the pool
        prefab = textPopupPrefab.gameObject;

        base.Awake();  // Calls the base class Awake to initialize the pool
    }

    /// <summary>
    /// Displays a text popup with the specified message, position, color, and duration.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="position">The position to display the text.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="duration">How long the text should be visible.</param>
    public void ShowTextPopup(string message, Vector3 position, Color color, float duration)
    {
        FloatingTextPopup popup = GetFromPool();
        popup.SetupPopup(message, position, color, duration);
        StartCoroutine(ReturnToPoolAfterDelay(popup, duration));
    }

    /// <summary>
    /// Coroutine that handles returning a text popup to the pool after a delay.
    /// </summary>
    /// <param name="textPopup">The text popup to return to the pool.</param>
    /// <param name="delay">The delay before returning to the pool.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    protected IEnumerator ReturnToPoolAfterDelay(FloatingTextPopup textPopup, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(textPopup);
    }
}
