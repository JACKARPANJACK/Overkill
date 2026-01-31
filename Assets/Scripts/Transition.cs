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
    public void animationFinished()
    {
        GameManager.Instance.NextLevel();
    }

    //call this function to start level transition
    public void startTransitionAnimation()
    {
        animator.enabled = true;
    }

}
