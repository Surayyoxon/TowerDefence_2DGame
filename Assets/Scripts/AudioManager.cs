using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("MUSIC")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSfx;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public float MusicVolume { get; private set; }
    public float GameVolume { get; private set; }

    // =====================================
    // UNITY
    // =====================================
    private void Awake()
    {
        // Sahna qayta yuklanganda bitta AudioManager qoladi (musiqa uzilmaydi)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        GameVolume = PlayerPrefs.GetFloat("GameVolume", 1f);

        musicSource.volume = MusicVolume;
        AudioListener.volume = GameVolume;
    }

    // =====================================
    // MUSIC
    // =====================================
    private void PlayMusic(AudioClip clip, bool loop)
    {
        if (clip == null)
            return;

        // Shu musiqa allaqachon chalinayotgan bo'lsa, qayta boshlamaydi
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusic, true);
    }

    public void PlayWinMusic()
    {
        PlayMusic(winMusic, false);
    }

    public void PlayGameOverMusic()
    {
        PlayMusic(gameOverMusic, false);
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    // =====================================
    // SFX
    // =====================================
    public void PlayClick()
    {
        PlaySFX(buttonClickSfx);
    }

    // Boshqa scriptlar uchun: AudioManager.Instance.PlaySFX(clip);
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    // =====================================
    // VOLUME
    // =====================================
    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        musicSource.volume = MusicVolume;

        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
    }

    public void SetGameVolume(float value)
    {
        GameVolume = Mathf.Clamp01(value);
        AudioListener.volume = GameVolume;

        PlayerPrefs.SetFloat("GameVolume", GameVolume);
    }
}