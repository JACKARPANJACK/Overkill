using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    private HUD h;
    private void Awake()
    {
        h = GetComponentInParent<HUD>();
    }

    public void onResumeBtn()
    {
        h.onPauseBtn();
    }

    public void onRestartBtn()
    {
        Debug.Log("Restart Button Pressed");
    }
}
