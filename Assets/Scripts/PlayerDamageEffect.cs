using UnityEngine;

/// <summary>
/// Controls the VignettePower parameter of a full screen shader material,
/// allowing smooth toggling between a max and an off value.
/// Starts in the "off" state.
/// </summary>
public class PlayerDamageEffect : MonoBehaviour
{
    [SerializeField] private Material vignetteMaterial;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float offValue = 10f;
    [SerializeField] private float offTransitionMultiplier = 2f;

    private float timer = 0f;
    private bool toggled = false;
    private float startValue;
    private float currentValue;
    private float autoTurnOffTime = 0f;

    void Start()
    {
        CacheStartValue();
        ApplyInstantValue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Toggle(!toggled, 2);
        }

        AnimateVignette();
    }

    /// <summary>
    /// Stores the initial VignettePower value to use as the "on" reference.
    /// </summary>
    private void CacheStartValue()
    {
        startValue = vignetteMaterial.GetFloat("_VignettePower");
        currentValue = offValue;
    }

    /// <summary>
    /// Public method to toggle the vignette effect on or off, with an optional duration, -1 makes it infinite.
    /// </summary>
    /// <param name="turnOn">True to turn on the effect, false to turn it off.</param>
    /// <param name="duration">Optional duration in seconds for how long the effect lasts before turning off automatically.</param>
    public void Toggle(bool turnOn, float duration = -1f)
    {
        if (toggled == turnOn)
            return;

        toggled = turnOn;
        timer = 0f;

        autoTurnOffTime = duration >= 0f ? duration : -1f;
    }



    /// <summary>
    /// Smoothly animates the VignettePower value based on toggle state.
    /// </summary>
    private void AnimateVignette()
    {
        if (transitionDuration <= 0f)
        {
            currentValue = toggled ? startValue : offValue;
            vignetteMaterial.SetFloat("_VignettePower", currentValue);
            return;
        }

        timer += Time.deltaTime;

        if (timer > transitionDuration)
            timer = transitionDuration;

        float t = Mathf.Clamp01(timer / transitionDuration);

        if (toggled)
        {
            currentValue = Mathf.Lerp(currentValue, startValue, t);
        }
        else
        {
            float slowT = Mathf.Clamp01(timer / (transitionDuration * offTransitionMultiplier));
            currentValue = Mathf.Lerp(currentValue, offValue, slowT);
        }

        vignetteMaterial.SetFloat("_VignettePower", currentValue);

        if (toggled && autoTurnOffTime > 0f)
        {
            autoTurnOffTime -= Time.deltaTime;
            if (autoTurnOffTime <= 0f)
            {
                Toggle(false);
            }
        }
    }

    /// <summary>
    /// Applies the target VignettePower immediately (no animation).
    /// </summary>
    private void ApplyInstantValue()
    {
        currentValue = toggled ? startValue : offValue;
        vignetteMaterial.SetFloat("_VignettePower", currentValue);
    }

    /// <summary>
    /// Resets the VignettePower to its original "on" value when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        vignetteMaterial.SetFloat("_VignettePower", startValue);
    }
}
