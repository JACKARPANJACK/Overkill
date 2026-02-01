using UnityEngine;
using UnityEngine.SceneManagement;



public class TitleScreen : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.cur_level = 1;

        AudioManager.Instance.PlayMusic(AudioManager.Instance.backgroundMusic);
    }
    public void onStartBtn()
    {
        SceneManager.LoadScene("level_1");
    }


    public void onControlsBtn()
    {
        Debug.Log("Controls Button");
    }

    public void toggleMusic(bool toggle)
    {
        if (toggle)
        {
            Debug.Log("Music Enabled");
        }
        else
        {
            Debug.Log("Music Disabled");
        }
    }

    public void onExitBtn()
    {
    #if (UNITY_EDITOR)

        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }
}
