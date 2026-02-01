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
            GameManager.Instance.ResumeGame();
            gameObject.SetActive(false);
    }

}
