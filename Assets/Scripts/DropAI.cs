using UnityEngine;

public class DropAI : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;


    private void showInfo()
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.showDialogue("Press Space to Drop Robo");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            showInfo();
            collision.GetComponent<Player>().inDropArea = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited drop zone");
            collision.GetComponent<Player>().inDropArea = false;
        }
    }

}
