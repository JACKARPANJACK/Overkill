using UnityEngine;
using UnityEngine.UI; // Required for UI

public class UploadConsole : MonoBehaviour
{
    [Header("Settings")]
    public float uploadSpeed = 10f; // How fast it goes up
    public Slider progressBar;      // Drag your UI Slider here
    public EndingManager endingManager; // Reference to the cutscene script

    private float currentProgress = 0f;
    private bool isComplete = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (isComplete) return;

        if (other.CompareTag("Player"))
        {
            // Increase Progress
            currentProgress += uploadSpeed * Time.deltaTime;

            // CLAMP at 99%
            if (currentProgress >= 99f)
            {
                currentProgress = 99f;
                TriggerEnding();
            }

            // Update UI
            if (progressBar) progressBar.value = currentProgress;
        }
    }

    void TriggerEnding()
    {
        isComplete = true;
        // Call the cinematic script
        endingManager.StartEndingSequence();
    }
}