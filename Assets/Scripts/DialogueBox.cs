using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject container; // contains dialogue box and Text UI element
    [SerializeField] private Text textElement; // Text UI element to display dialogue
    public void showDialogue(string text)
    {
        container.SetActive(true);
        textElement.text = text;
    }

    public void hideDialogueBox() => container.SetActive(false);
    
}
