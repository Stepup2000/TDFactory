using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    // Singleton instance
    private static FloatingTextController _instance;

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
    private FloatingTextPopup textPopupPrefab;

    [SerializeField, Tooltip("Gameobject to hold the floating text popups.")]
    private GameObject textPopupContainer;

    [SerializeField, Tooltip("Initial size of the text popup pool.")]
    private int initialPoolSize = 10;

    [SerializeField, Tooltip("Maximum size the text popup pool can grow to.")]
    private int maxPoolSize = 20;

    private List<FloatingTextPopup> textPopupPool = new List<FloatingTextPopup>();

    /// <summary>
    /// Called when the script is enabled. Sets up the singleton instance and initializes the text popup pool.
    /// </summary>
    private void Awake()
    {
        // Ensure this instance is the only one, destroy duplicates
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate FloatingTextController instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitializePool(); // Initialize the text popup pool
    }

    /// <summary>
    /// Initializes the text popup pool with a predefined number of objects.
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewTextPopup(); // Create new text popups and add to pool
        }
    }

    /// <summary>
    /// Retrieves or creates the container for text popup objects.
    /// </summary>
    /// <returns>The text popup container GameObject.</returns>
    private GameObject GetTextPopupContainer()
    {
        if (textPopupContainer == null)
        {
            // Create a new container if it does not exist
            textPopupContainer = new GameObject("TextPopupContainer");
            textPopupContainer.transform.SetParent(_instance.transform);
        }
        return textPopupContainer;
    }

    /// <summary>
    /// Retrieves an available text popup from the pool.
    /// </summary>
    /// <returns>An available text popup, or null if none are available.</returns>
    private FloatingTextPopup GetAvailableTextPopup()
    {
        foreach (FloatingTextPopup popup in textPopupPool)
        {
            if (!popup.gameObject.activeSelf)
                return popup;
        }
        return null;
    }

    /// <summary>
    /// Creates a new text popup and adds it to the pool, provided the pool size limit has not been reached.
    /// </summary>
    private void CreateNewTextPopup()
    {
        if (textPopupPool.Count >= maxPoolSize)
        {
            Debug.LogWarning("TextPopup pool limit has been reached, not creating a new one");
            return;
        }
            

        if (textPopupPrefab == null)
        {
            Debug.LogWarning("TextPopup prefab is null, returning");
            return;
        }

        FloatingTextPopup newTextPopup = Instantiate<FloatingTextPopup>(textPopupPrefab, Vector3.zero, Quaternion.identity);
        textPopupPool.Add(newTextPopup);
        newTextPopup.transform.SetParent(GetTextPopupContainer().transform);
        newTextPopup.gameObject.SetActive(false); // Initially inactive
    }

    /// <summary>
    /// Coroutine that handles returning a text popup to the pool after a delay.
    /// </summary>
    /// <param name="textPopup">The text popup to return to the pool.</param>
    /// <param name="delay">The delay before returning to the pool.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator ReturnToPool(FloatingTextPopup textPopup, float delay)
    {
        yield return new WaitForSeconds(delay);
        textPopup.StopMoving(); // Stop any movement or animation
        textPopup.gameObject.SetActive(false); // Return text popup to pool
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
        FloatingTextPopup popup = GetAvailableTextPopup();
        if (popup == null)
        {
            CreateNewTextPopup();
            popup = GetAvailableTextPopup();
        }

        if (popup != null)
        {
            popup.gameObject.SetActive(true);
            popup.SetupPopup(message, position, color, duration);
            StartCoroutine(ReturnToPool(popup, duration));
        }
    }
}
