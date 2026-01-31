using UnityEngine;

public class DisplayScreen : MonoBehaviour
{
    private Vector2[] screenResolution = new Vector2[]
    {
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080)
    };

    public void onResolutionBtn(int index)
    {
        Vector2 resolution = screenResolution[index];
        Screen.SetResolution((int)resolution.x, (int)resolution.y, Screen.fullScreen);
    }
}
