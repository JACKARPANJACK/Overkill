using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Controller : MonoBehaviour
{
    
    public void Awake()
    {
        Debug.Log(GameManager.Instance.score);
    }

    private void change_scene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }

    //All UI Button Functions Below
    public void onStartBtn()
    {
        change_scene("SampleScene");
    }

    public void onDisplayBtn()
    {
        change_scene("DisplayScreen");

    }

    public void OnAboutBtn()
    {
        change_scene("AboutUsScreen");
    }

    public void onControlsBtn()
    {
        change_scene("ControlsScreen");
    }

    public void BackToTitle()
    {
        change_scene("TitleScreen");
    }

    public void BackToOptions()
    {
        change_scene("OptionsScreen");
    }


    public void toggleMusic(bool toggle)
    {
        if (toggle)
            Debug.Log("Music Enabled");
        else
            Debug.Log("Music Disabled");
    }

    public void onExitBtn()
    {
    #if (UNITY_EDITOR)

        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }
}
