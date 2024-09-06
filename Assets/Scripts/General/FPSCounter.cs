using UnityEngine;

/// <summary>
/// This script calculates and logs the average FPS over a specified duration (30 seconds).
/// </summary>
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private float totalTime = 30f; // Duration in seconds over which to calculate average FPS

    private float timeElapsed = 0f; // Time elapsed since last calculation
    private int frameCount = 0; // Number of frames counted
    private float fpsSum = 0f; // Sum of FPS values over the period

    /// <summary>
    /// Updates the FPS counter every frame.
    /// </summary>
    void Update()
    {
        timeElapsed += Time.deltaTime; // Accumulate time
        frameCount++; // Increment frame count
        fpsSum += 1 / Time.deltaTime; // Add the current frame's FPS to the sum

        // Check if the total time has elapsed
        if (timeElapsed >= totalTime)
        {
            float averageFPS = fpsSum / frameCount; // Calculate average FPS
            Debug.Log("Average FPS over the last " + totalTime + " seconds: " + averageFPS);

            // Reset for the next calculation
            timeElapsed = 0f;
            frameCount = 0;
            fpsSum = 0f;
        }
    }
}
