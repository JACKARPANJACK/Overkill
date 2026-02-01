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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.BtnClick);
        SceneManager.LoadScene(scene_name);
    }


    //All UI Button Functions Below
    public void onStartBtn()
    {
        change_scene("level_1");
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
    public void RestartLevel()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.BtnClick);
        GameManager.Instance.RestartLevel();
    }

    public void toggleMusic(bool toggle)
    {
        
        GameManager.Instance.musicOn = toggle;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.BtnClick);
    }

    public void onExitBtn()
    {
    #if (UNITY_EDITOR)

        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }
}
