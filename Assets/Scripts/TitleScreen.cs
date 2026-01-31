using UnityEngine;



public class TitleScreen : MonoBehaviour
{

    public void onStartBtn()
    {
        Debug.Log("Start Button Pressed");
    }


    public void onControlsBtn()
    {
        Debug.Log("Controls Button Pressed");
    }

    public void toggleMusic(bool toggle)
    {
        if (toggle)
        {
            // Enable music
            Debug.Log("Music Enabled");
        }
        else
        {
            // Disable music
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
