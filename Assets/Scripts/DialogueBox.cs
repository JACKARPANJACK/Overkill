using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueBox : MonoBehaviour
{
    [SerializeField] private GameObject container; // contains dialogue box and Text UI element
    [SerializeField] private Text textElement; // Text UI element to display dialogue
    private float lastDialogueTime = 0f;
    private bool isDialogueActive = false;


    public void showDialogue(string text)
    {
        Debug.Log("Showing Dialogue: " + text);
        container.SetActive(true);
        lastDialogueTime = Time.time;
        textElement.text = text;
        isDialogueActive = true;

    }

    public void hideDialogueBox()
    {
        container.SetActive(false);
        isDialogueActive = false;
    }
    void Update()
    {
        if(isDialogueActive)
        {
            //if dialogue is active for more than 3 seconds then hide it
            if (Time.time - lastDialogueTime > 3f)
            {
                hideDialogueBox();
                isDialogueActive = false;
            }
        }

    }
}

