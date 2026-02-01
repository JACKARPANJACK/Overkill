using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isPaused=false;
    public int score=0;
    public int cur_level=1; // current level
    public int cur_weaponIdx = 0; // current weapon index
    public bool isGameOver = false;
    public bool musicOn = true;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        DontDestroyOnLoad(gm);
    }

    private void Awake()
    {
        // Hard singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
    }  

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
    }

    public void NextLevel()
    {
        cur_level++;
        //SceneManager.LoadScene("Level_" + cur_level);
        SceneManager.LoadScene("SampleScene");
    }

    public void updateScore(int value) => score += value;
}
