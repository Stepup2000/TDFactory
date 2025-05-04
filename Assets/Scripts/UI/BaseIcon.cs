using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BaseIcon : MonoBehaviour, IPoolable
{
    [Header("Scale (Breathing) Settings")]
    [SerializeField] private bool scaleBreathingEnabled = true;
    [SerializeField] private float scaleAmplitude = 0.1f;
    [SerializeField] private float scaleSpeed = 2f;
    private Vector3 originalScale;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotationEnabled = true;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float rotationSpeed = 45f;
    public Image iconImage;

    private void Awake()
    {
        // Initialize original scale and icon image reference
        originalScale = transform.localScale;
        iconImage = GetComponent<Image>();
    }

    private void Update()
    {
        // Continuously handle breathing and rotation effects during the update cycle
        HandleBreathing();
        HandleRotation();
    }

    /// <summary>
    /// Updates the scale based on the breathing effect.
    /// </summary>
    private void HandleBreathing()
    {
        if (scaleBreathingEnabled)
        {
            float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
            transform.localScale = originalScale + Vector3.one * scaleOffset;
        }
    }

    /// <summary>
    /// Rotates the icon based on the rotation settings.
    /// </summary>
    private void HandleRotation()
    {
        if (rotationEnabled)
        {
            transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    /// <summary>
    /// Enables or disables the breathing effect.
    /// </summary>
    public void EnableBreathing(bool enable)
    {
        scaleBreathingEnabled = enable;
        if (!enable)
            transform.localScale = originalScale;
    }

    /// <summary>
    /// Enables or disables the rotation effect.
    /// </summary>
    public void EnableRotation(bool enable)
    {
        rotationEnabled = enable;
    }

    /// <summary>
    /// Resets the icon when it spawns (restores default scale and rotation).
    /// </summary>
    public void OnSpawn()
    {
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;

        scaleBreathingEnabled = true;
        rotationEnabled = true;
    }

    /// <summary>
    /// Disables the breathing and rotation effects when the icon is despawned.
    /// </summary>
    public void OnDespawn()
    {
        scaleBreathingEnabled = false;
        rotationEnabled = false;
    }

    /// <summary>
    /// Resets the icon to its original state, including scale and rotation.
    /// </summary>
    public void ResetObject()
    {
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;

        scaleBreathingEnabled = true;
        rotationEnabled = true;
    }
}
