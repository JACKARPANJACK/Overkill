using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip backgroundMusic;
    public AudioClip BtnClick;
    public AudioClip GlassBreak;
    public AudioClip BulletFire;
    public AudioClip Explosion;
    public AudioClip ToxicGasLeak;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if(GameManager.Instance.musicOn == false) return;
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if(GameManager.Instance.musicOn == false) return;   
        sfxSource.PlayOneShot(clip);
    }

}

