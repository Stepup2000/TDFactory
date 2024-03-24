using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float totalTime = 30f;
    private float timeElapsed = 0f;
    private int frameCount = 0;
    private float totalFPS = 0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        frameCount++;
        totalFPS += 1 / Time.deltaTime;

        if (timeElapsed >= totalTime)
        {
            float averageFPS = totalFPS / frameCount;
            Debug.Log("Average FPS over the last 30 seconds: " + averageFPS);

            // Reset variables for next calculation
            timeElapsed = 0f;
            frameCount = 0;
            totalFPS = 0f;
        }
    }
}
