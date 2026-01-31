using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{

    public void onStartBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void onDisplayBtn()
    {
        SceneManager.LoadScene("DisplayScreen");

    }

    public void OnAboutBtn()
    {
        SceneManager.LoadScene("AboutUsScreen");
    }

    public void onControlsBtn()
    {
        SceneManager.LoadScene("ControlsScreen");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BackToOptions()
    {
        SceneManager.LoadScene("OptionsScreen");
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
