using UnityEngine;
using System.Collections;

public class IconDisplayManager : BaseObjectPooler<BaseIcon>
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

    protected override void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate IconDisplayManager instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        if (worldCanvas == null)
        {
            Debug.LogError("Missing canvas reference in IconDisplayManager. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializePool();
    }

    /// <summary>
    /// Displays an icon at the specified world position for a limited duration.
    /// </summary>
    /// <param name="sprite">The sprite to display on the icon.</param>
    /// <param name="worldPosition">The world position to display the icon at.</param>
    /// <param name="duration">How long the icon should remain visible (in seconds).</param>
    public void ShowIcon(Sprite sprite, Vector3 worldPosition, float duration = 2f, bool isRotating = false, bool isBreathing = false)
    {
        if (worldCanvas == null)
        {
            Debug.LogError("IconDisplayManager missing canvas reference.");
            return;
        }

        BaseIcon icon = GetFromPool();

        icon.transform.position = worldPosition;

        if (sprite != null)
            icon.iconImage.sprite = sprite;

        icon.EnableRotation(isRotating);
        icon.EnableBreathing(isBreathing);

        icon.gameObject.SetActive(true);

        StartCoroutine(HideAfterDelay(icon, duration));
    }

    /// <summary>
    /// Hides the icon after a delay.
    /// </summary>
    /// <param name="icon">The icon to hide.</param>
    /// <param name="delay">Time in seconds before the icon is hidden.</param>
    private IEnumerator HideAfterDelay(BaseIcon icon, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(icon);
    }
}
