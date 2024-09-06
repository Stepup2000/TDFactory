using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the visual flash effect for an enemy when it takes damage. 
/// Changes the material of the enemy's renderers temporarily to indicate a hit.
/// </summary>
public class HitFlash : MonoBehaviour
{
    [SerializeField] private Material _hitMaterial; // The material to use for the hit flash effect.
    [SerializeField] private float _flashDuration = 0.05f; // Duration of the hit flash effect.

    private Material[] _originalMaterials; // Array to store the original materials of the renderers.
    private Renderer[] _renderers; // Array of renderers attached to this GameObject and its children.
    private BaseEnemy _targetEnemy; // Reference to the BaseEnemy component to subscribe to health changes.

    /// <summary>
    /// Called when the script is enabled. Subscribes to the OnHealthChanged event of the target enemy.
    /// </summary>
    private void OnEnable()
    {
        // Attempt to get the BaseEnemy component attached to this GameObject
        if (TryGetComponent<BaseEnemy>(out _targetEnemy))
        {
            // Subscribe to the OnHealthChanged event
            _targetEnemy.OnHealthChanged += TriggerHitEffect;
        }
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the OnHealthChanged event to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        // Unsubscribe from the OnHealthChanged event if the targetEnemy is set
        if (_targetEnemy != null)
        {
            _targetEnemy.OnHealthChanged -= TriggerHitEffect;
        }
    }

    /// <summary>
    /// Initializes the renderer components and stores their original materials.
    /// </summary>
    void Start()
    {
        // Retrieve all Renderer components in this GameObject and its children
        _renderers = GetComponentsInChildren<Renderer>();
        _originalMaterials = new Material[_renderers.Length];

        // Store the original material for each renderer
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].material;
        }
    }

    /// <summary>
    /// Triggers the hit effect by starting a coroutine to flash the material.
    /// </summary>
    /// <param name="newValue">The current health value of the enemy (not used in this method).</param>
    /// <param name="maxValue">The maximum health value of the enemy (not used in this method).</param>
    public void TriggerHitEffect(float newValue, float maxValue)
    {
        // Start the coroutine to handle the hit effect
        StartCoroutine(HitEffectCoroutine(_flashDuration));
    }

    /// <summary>
    /// Coroutine that handles the flash effect by temporarily changing the material to the hit material,
    /// and then reverting to the original materials after the specified duration.
    /// </summary>
    /// <param name="duration">The duration for which the hit material will be displayed.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator HitEffectCoroutine(float duration)
    {
        // Swap to hit material
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _hitMaterial;
        }

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Revert to original material
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _originalMaterials[i];
        }
    }
}
