using UnityEngine;

public class DisplayScreen : MonoBehaviour
{
    private Vector2[] screenResolution = new Vector2[]
    {
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080)
    };
    private Vector2 selectedResolution;

    public void onResolutionBtn(int index)
    {
        selectedResolution = screenResolution[index];
        
    }

    public void applySettings()
    {
        Screen.SetResolution((int)selectedResolution.x, (int)selectedResolution.y, Screen.fullScreen);
    }
}
