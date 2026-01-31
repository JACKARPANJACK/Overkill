using UnityEngine;

public class HUD : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] private PauseScreen pauseScreen;


    private void Awake()
    {
        pauseScreen.gameObject.SetActive(isPaused);
    }
    public void onPauseBtn()
    {
        if (!isPaused)
        { 
            Time.timeScale = 0;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
        }
        //Pause Screen will only be active when isPaused is true
        pauseScreen.gameObject.SetActive(isPaused);

    }
}
