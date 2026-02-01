using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{ 
    [SerializeField] private PauseScreen pauseScreen;
    [SerializeField] private GameObject weaponDisplay;
    [SerializeField]Sprite[] weapons = new Sprite[3];
    private void Awake()
    {
        pauseScreen.gameObject.SetActive(GameManager.Instance.isPaused);
    }
    public void onPauseBtn()
    {
        GameManager.Instance.PauseGame();
        //Pause Screen will only be active when isPaused is true
        pauseScreen.gameObject.SetActive(GameManager.Instance.isPaused);

    }

    public void NextWeapon()
    {
        if(GameManager.Instance.cur_weaponIdx < weapons.Length - 1)
            GameManager.Instance.cur_weaponIdx++;
        else
            GameManager.Instance.cur_weaponIdx = 0;

        weaponDisplay.GetComponent<Image>().sprite = weapons[GameManager.Instance.cur_weaponIdx];


    }

    public void PreviousWeapon()
    {
        if (GameManager.Instance.cur_weaponIdx > 0)
            GameManager.Instance.cur_weaponIdx--;
        else
            GameManager.Instance.cur_weaponIdx = weapons.Length - 1;

        weaponDisplay.GetComponent<Image>().sprite = weapons[GameManager.Instance.cur_weaponIdx];
    }


    //for testing purpose only
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            NextWeapon();
            }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
                PreviousWeapon();
        }

    }

}
