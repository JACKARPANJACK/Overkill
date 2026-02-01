using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{ 
    [SerializeField] private PauseScreen pauseScreen;
    [SerializeField] private GameObject weaponDisplay;
    [SerializeField] private Sprite[] weapons = new Sprite[3];
    [SerializeField] private Text ScoreText;

    private void Awake()
    {
        pauseScreen.gameObject.SetActive(GameManager.Instance.isPaused);
        ScoreText.text = "Score: " + GameManager.Instance.score;
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

    private void updateScoreText()
    {
        ScoreText.text = "Score: " + GameManager.Instance.score;
    }

    //for testing purpose only
    void Update()
    {
        if(Keyboard.current.escapeKey.isPressed)
        {
            if (GameManager.Instance.isPaused)
            {
                GameManager.Instance.ResumeGame();
                pauseScreen.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.PauseGame();
                pauseScreen.gameObject.SetActive(true);
            }
        }

        updateScoreText();

    }

}
