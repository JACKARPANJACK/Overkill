using UnityEngine;
using UnityEngine.UI;

public class optionScreen : MonoBehaviour
{
    private Toggle toggle;
    private void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        toggle.isOn = GameManager.Instance.musicOn;
    }
}
