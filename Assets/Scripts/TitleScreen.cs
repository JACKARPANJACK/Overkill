using UnityEngine;



public class TitleScreen : MonoBehaviour
{

    public void onStartBtn()
    {
        Debug.Log("Start Button");
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
