using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Use TextMeshPro if you have it, or standard Text

public class EndingManager : MonoBehaviour
{
    [Header("Actors")]
    public GameObject companionAI; // Drag your AI Robot here
    public Transform dropZone;     // The center of the core (Target for AI)
    public Transform safeZone;     // Where player stands
    public GameObject coreExplosionVFX;

    [Header("UI")]
    public GameObject dialogueBox; // Panel with Text
    public TextMeshProUGUI dialogueText; // The actual text component
    public GameObject winScreen;

    public void StartEndingSequence()
    {
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        // 1. DISABLE CONTROLS
        // Find Player and AI scripts and disable them so they stop moving/shooting
        companionAI.GetComponent<MonoBehaviour>().enabled = false;
        // player.GetComponent<PlayerController>().enabled = false; (Optional)

        // 2. DIALOGUE: The Problem
        dialogueBox.SetActive(true);
        dialogueText.text = "AI: UPLOAD ERROR. HARDWARE LOCK DETECTED.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "AI: THE BLOCKAGE IS... INTERNAL.";
        yield return new WaitForSeconds(3f);

        dialogueText.text = "AI: PROTOCOL UPDATE: SACRIFICE.";
        yield return new WaitForSeconds(3f);

        // 3. ACTION: AI Moves to Drop Zone
        dialogueBox.SetActive(false);

        // Simple move logic for cutscene
        float t = 0;
        Vector3 startPos = companionAI.transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f; // Move slowly
            companionAI.transform.position = Vector3.Lerp(startPos, dropZone.position, t);
            yield return null;
        }

        // 4. ACTION: Explosion
        Instantiate(coreExplosionVFX, dropZone.position, Quaternion.identity);
        Destroy(companionAI); // Kill the AI

        // Screenshake here if you have it
        yield return new WaitForSeconds(2f);

        // 5. FINAL MONOLOGUE (The Scientist)
        dialogueBox.SetActive(true);
        dialogueText.color = Color.yellow; // Change color for player voice

        dialogueText.text = "Scientist: I wanted to solve the world's problems...";
        yield return new WaitForSeconds(4f);

        dialogueText.text = "Scientist: I made a god to save us.";
        yield return new WaitForSeconds(4f);

        dialogueText.text = "Scientist: But power without a soul... is just a bomb.";
        yield return new WaitForSeconds(5f);

        dialogueText.text = "Scientist: Goodbye, old friend.";
        yield return new WaitForSeconds(3f);

        // 6. WIN SCREEN
        dialogueBox.SetActive(false);
        winScreen.SetActive(true);
    }
}