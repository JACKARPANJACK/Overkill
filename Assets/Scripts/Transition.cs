using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Transition : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    //public void Start()
    //{
    //   startTransitionAnimation();
    //}
    public void animationFinished()
    {
        Debug.Log("Transition Animation Finished");
        GameManager.Instance.NextLevel();

    }

    //call this function to start level transition
    public void startTransitionAnimation()
    {
        animator.enabled = true;
    }

}
