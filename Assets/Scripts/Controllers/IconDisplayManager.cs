using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class IconDisplayManager : MonoBehaviour
{
    private static IconDisplayManager instance;

    public static IconDisplayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<IconDisplayManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("IconDisplayManager");
                    instance = obj.AddComponent<IconDisplayManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private int initialPoolSize = 10;

    private List<GameObject> iconPool = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate IconDisplayManager instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        if (worldCanvas == null || iconPrefab == null)
        {
            Debug.LogError("Missing canvas or prefab reference in IconDisplayManager.  Destroying this instance");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    /// <summary>
    /// Creates a pool of inactive icon instances.
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject icon = Instantiate(iconPrefab, worldCanvas.transform);
            icon.SetActive(false);
            iconPool.Add(icon);
        }
    }

    /// <summary>
    /// Returns an inactive icon from the pool, or instantiates a new one if none are available.
    /// </summary>
    private GameObject GetPooledIcon()
    {
        foreach (var icon in iconPool)
        {
            if (!icon.activeInHierarchy)
                return icon;
        }

        // Pool limit reached — expand pool
        GameObject newIcon = Instantiate(iconPrefab, worldCanvas.transform);
        newIcon.SetActive(false);
        iconPool.Add(newIcon);
        Debug.Log("Pool expanded: total count " + iconPool.Count);
        return newIcon;
    }

    /// <summary>
    /// Displays an icon at the specified world position for a limited duration.
    /// </summary>
    /// <param name="sprite">The sprite to display on the icon.</param>
    /// <param name="worldPosition">The world position to display the icon at.</param>
    /// <param name="duration">How long the icon should remain visible (in seconds).</param>
    public void ShowIcon(Sprite sprite, Vector3 worldPosition, float duration = 2f)
    {
        if (worldCanvas == null || iconPrefab == null)
        {
            Debug.LogError("IconDisplayManager missing canvas or prefab reference.");
            return;
        }

        GameObject icon = GetPooledIcon();
        icon.transform.position = worldPosition;
        if (sprite != null) icon.GetComponent<Image>().sprite = sprite;
        icon.SetActive(true);

        StartCoroutine(HideAfterDelay(icon, duration));
    }

    /// <summary>
    /// Hides the icon after a delay.
    /// </summary>
    /// <param name="icon">The icon to hide.</param>
    /// <param name="delay">Time in seconds before the icon is hidden.</param>
    private IEnumerator HideAfterDelay(GameObject icon, float delay)
    {
        yield return new WaitForSeconds(delay);
        icon.SetActive(false);
    }
}
