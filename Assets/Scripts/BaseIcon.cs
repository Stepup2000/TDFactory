using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIcon : MonoBehaviour
{
    [Header("Scale (Breathing) Settings")]
    [SerializeField] private bool scaleBreathingEnabled = true;
    [SerializeField] private float scaleAmplitude = 0.1f;
    [SerializeField] private float scaleSpeed = 2f;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotationEnabled = true;
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float rotationSpeed = 45f;

    private void OnEnable()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    private void OnDisable()
    {
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }

    /// <summary>
    /// Handles breathing and rotation updates each frame.
    /// </summary>
    private void Update()
    {
        HandleBreathing();
        HandleRotation();
    }

    /// <summary>
    /// Updates the object's scale to create a breathing (pulsing) effect.
    /// </summary>
    private void HandleBreathing()
    {
        if (!scaleBreathingEnabled) return;

        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
        transform.localScale = originalScale + Vector3.one * scaleOffset;
    }

    /// <summary>
    /// Continuously rotates the object if enabled.
    /// </summary>
    private void HandleRotation()
    {
        if (!rotationEnabled) return;

        transform.Rotate(rotationAxis.normalized, rotationSpeed * Time.deltaTime, Space.Self);
    }

    /// <summary>
    /// Enables or disables the breathing (scaling) effect.
    /// </summary>
    /// <param name="enable">True to enable, false to disable.</param>
    public void EnableBreathing(bool enable)
    {
        scaleBreathingEnabled = enable;

        if (!enable)
            transform.localScale = originalScale;
    }

    /// <summary>
    /// Enables or disables continuous rotation.
    /// </summary>
    /// <param name="enable">True to enable, false to disable.</param>
    public void EnableRotation(bool enable)
    {
        rotationEnabled = enable;
    }
}
