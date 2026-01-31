using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    private HUD hud;
    private void Awake()
    {
        hud = GetComponentInParent<HUD>();
    }

    public void onResumeBtn()
    {
        hud.onPauseBtn();
    }

    public void onRestartBtn()
    {
        Debug.Log("Restart");
    }
}
