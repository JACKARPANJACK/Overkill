using System.Xml.Serialization;
using UnityEngine;

public class TransitionObject : MonoBehaviour
{

    [SerializeField] Transition TransitionArea;
    private void Start()
    {
        Debug.Log("TransitionObject Initialized");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TransitionObject Triggered");
        if (collision.GetComponent<Player>())
        {
          TransitionArea.startTransitionAnimation();
        }
    }
}
