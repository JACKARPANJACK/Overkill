using UnityEngine;

public class HUD : MonoBehaviour
{ 
    [SerializeField] private PauseScreen pauseScreen;


    private void Awake()
    {
        pauseScreen.gameObject.SetActive(GameManager.Instance.isPaused);
    }
    public void onPauseBtn()
    {
        GameManager.Instance.PauseGame();
        //Pause Screen will only be active when isPaused is true
        pauseScreen.gameObject.SetActive(GameManager.Instance.isPaused);

    }
}
