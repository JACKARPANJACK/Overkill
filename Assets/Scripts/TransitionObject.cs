using System.Xml.Serialization;
using UnityEngine;

public class TransitionObject : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TransitionObject Triggered");
        if (collision.CompareTag("Player"))
        {
          transform.parent.GetComponentInChildren<Transition>().startTransitionAnimation();
        }
    }
}
