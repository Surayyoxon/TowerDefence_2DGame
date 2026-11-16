
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("========== PANELS ==========")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject youDiedPanel;

    [Header("========== GAME UI ==========")]
    [SerializeField] private Slider baseHealthBar;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text waveText;

    [Header("Enemy HP Bars")]
    [SerializeField] private Image[] enemyHPBars = new Image[4];

    [Header("========== TOWER BUTTONS ==========")]
    [SerializeField] private Button[] towerActiveButtons = new Button[4];
    [SerializeField] private GameObject[] towerInactiveButtons = new GameObject[4];
    [SerializeField] private int[] towerPrices = new int[4];

    [Header("Tower Build Manager")]
    [SerializeField] private TowerBuildManager towerBuildManager;

    [Header("========== SETTINGS ==========")]
    [SerializeField] private Slider[] musicSliders = new Slider[2];
    [SerializeField] private Slider[] volumeSliders = new Slider[2];

    [SerializeField] private AudioSource musicSource;

    [Header("========== GAME SETTINGS ==========")]
    [SerializeField] private int startingCoins = 500;
    [SerializeField] private int totalWaves = 5;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string gameSceneName;

    private int currentCoins;
    private int currentWave = 1;

    private float baseMaxHealth = 100f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        currentCoins = startingCoins;
        currentWave = 1;

        InitializeSettings();
        UpdateCoinUI();
        UpdateWaveUI();
        UpdateTowerButtons();

        SetBaseHealth(baseMaxHealth, baseMaxHealth);

        ShowMainMenu();
    }

    // =====================================
    // MAIN MENU
    // =====================================

    public void ShowMainMenu()
    {
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        settingsPanel.SetActive(false);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(false);
        youDiedPanel.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(gameSceneName) &&
            SceneManager.GetActiveScene().name != gameSceneName)
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(true);

        ResetGameUI();
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");

        Application.Quit();
    }

    // =====================================
    // SETTINGS PANEL
    // =====================================

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void SettingsBack()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // =====================================
    // PAUSE PANEL
    // =====================================

    public void PauseGame()
    {
        Time.timeScale = 0f;

        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void PauseMainMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            ShowMainMenu();
        }
    }

    // =====================================
    // WIN PANEL
    // =====================================

    public void ShowWinPanel()
    {
        Time.timeScale = 0f;

        gamePanel.SetActive(false);
        youWinPanel.SetActive(true);
    }

    public void ReplayLevel()
    {
        RestartLevel();
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextSceneIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Next level scene not found!");
        }
    }

    // =====================================
    // GAME OVER PANEL
    // =====================================

    public void ShowDiedPanel()
    {
        Time.timeScale = 0f;

        gamePanel.SetActive(false);
        youDiedPanel.SetActive(true);
    }

    public void RetryLevel()
    {
        RestartLevel();
    }

    // =====================================
    // COIN SYSTEM
    // =====================================

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        UpdateCoinUI();
        UpdateTowerButtons();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
        {
            Debug.Log("Not enough coins!");
            return false;
        }

        currentCoins -= amount;

        UpdateCoinUI();
        UpdateTowerButtons();

        return true;
    }

    public int GetCoins()
    {
        return currentCoins;
    }

    private void UpdateCoinUI()
    {
        coinText.text = currentCoins.ToString();
    }

    // =====================================
    // TOWER PURCHASE SYSTEM
    // =====================================

    private void UpdateTowerButtons()
    {
        for (int i = 0; i < towerActiveButtons.Length; i++)
        {
            bool canBuy = currentCoins >= towerPrices[i];

            towerActiveButtons[i].gameObject.SetActive(canBuy);

            towerInactiveButtons[i].SetActive(!canBuy);
        }
    }

    public void SelectTower(int towerIndex)
    {
        if (towerIndex < 0 ||
            towerIndex >= towerPrices.Length)
            return;

        if (currentCoins < towerPrices[towerIndex])
        {
            Debug.Log("Not enough coins for this tower!");
            return;
        }

        if (towerBuildManager != null)
        {
            towerBuildManager.SelectTower(towerIndex);
        }
    }

    public bool BuyTower(int towerIndex)
    {
        if (towerIndex < 0 ||
            towerIndex >= towerPrices.Length)
            return false;

        return SpendCoins(towerPrices[towerIndex]);
    }

    // =====================================
    // BASE HEALTH SYSTEM
    // =====================================

    public void SetBaseHealth(float currentHealth, float maxHealth)
    {
        baseHealthBar.value =
            Mathf.Clamp01(currentHealth / maxHealth);
    }

    // =====================================
    // ENEMY HEALTH BARS
    // =====================================

    public void SetEnemyHealth(
        int enemyIndex,
        float currentHealth,
        float maxHealth)
    {
        if (enemyIndex < 0 ||
            enemyIndex >= enemyHPBars.Length)
            return;

        if (enemyHPBars[enemyIndex] == null)
            return;

        enemyHPBars[enemyIndex].fillAmount =
            Mathf.Clamp01(currentHealth / maxHealth);
    }

    public void HideEnemyHealth(int enemyIndex)
    {
        if (enemyIndex < 0 ||
            enemyIndex >= enemyHPBars.Length)
            return;

        enemyHPBars[enemyIndex].gameObject.SetActive(false);
    }

    public void ShowEnemyHealth(int enemyIndex)
    {
        if (enemyIndex < 0 ||
            enemyIndex >= enemyHPBars.Length)
            return;

        enemyHPBars[enemyIndex].gameObject.SetActive(true);
    }

    // =====================================
    // WAVE SYSTEM
    // =====================================

    public void SetWave(int wave)
    {
        currentWave = Mathf.Clamp(wave, 1, totalWaves);

        UpdateWaveUI();
    }

    public void UpdateWaveUI()
    {
        waveText.text = "WAVE " +
                        currentWave + " / " +
                        totalWaves;
    }

    // =====================================
    // AUDIO SETTINGS
    // =====================================

    private void InitializeSettings()
    {
        float savedMusic =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        float savedVolume =
            PlayerPrefs.GetFloat("GameVolume", 1f);

        foreach (Slider slider in musicSliders)
        {
            if (slider == null) continue;

            slider.SetValueWithoutNotify(savedMusic);
            slider.onValueChanged.AddListener(SetMusicVolume);
        }

        foreach (Slider slider in volumeSliders)
        {
            if (slider == null) continue;

            slider.SetValueWithoutNotify(savedVolume);
            slider.onValueChanged.AddListener(SetGameVolume);
        }

        SetMusicVolume(savedMusic);
        SetGameVolume(savedVolume);
    }

    public void SetMusicVolume(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
        }

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetGameVolume(float value)
    {
        AudioListener.volume = value;

        PlayerPrefs.SetFloat("GameVolume", value);
    }

    // =====================================
    // RESET UI
    // =====================================

    public void ResetGameUI()
    {
        currentCoins = startingCoins;
        currentWave = 1;

        UpdateCoinUI();
        UpdateWaveUI();
        UpdateTowerButtons();

        SetBaseHealth(baseMaxHealth, baseMaxHealth);

        foreach (Image hpBar in enemyHPBars)
        {
            if (hpBar != null)
                hpBar.fillAmount = 1f;
        }
    }
}
