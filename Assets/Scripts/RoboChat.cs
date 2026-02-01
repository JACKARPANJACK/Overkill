using Unity.VisualScripting;
using UnityEngine;

public class RoboChat : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;

    public void chat(string message)
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.showDialogue(message);
    }
}
