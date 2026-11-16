using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // Restart / Retry / Replay bosilganda menyuni o'tkazib, o'yinni darhol boshlash uchun
    private static bool startImmediately = false;

    // =====================================
    // PANELS
    // =====================================
    [Header("PANELS")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject youDiedPanel;

    // =====================================
    // BUTTONS (Inspector'da ulanmaydi - panel ichidan nomi bo'yicha topiladi)
    // =====================================
    private Button startButton, settingsButton, quitButton;
    private Button pauseButton;
    private Button settingsBackButton;
    private Button pauseRestartButton, pauseMainMenuButton, pauseBackButton;
    private Button winReplayButton, winMainMenuButton, winNextButton;
    private Button diedRetryButton, diedMainMenuButton;

    // =====================================
    // GAME UI
    // =====================================
    [Header("GAME UI")]
    [SerializeField] private Image baseHealthFill;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text waveText;

    // Enemy HP barlar shu konteyner ichida yaratiladi (Game Panel ichida,
    // shuning uchun pause / win / died / menu paytida avtomatik yashirinadi)
    public RectTransform EnemyBarContainer { get; private set; }

    // =====================================
    // TOWER BUTTONS
    // =====================================
    [Header("TOWER BUTTONS")]
    private Button[] towerActiveButtons = new Button[4];
    private GameObject[] towerInactiveButtons = new GameObject[4];
    [SerializeField] private int[] towerPrices = new int[4];

    [Header("Managers")]
    [SerializeField] private TowerBuildManager towerBuildManager;

    // =====================================
    // AUDIO SLIDERS (Settings va Pause panelidan VolumeSlider'lar avtomatik topiladi)
    // =====================================
    private readonly List<VolumeSlider> musicSliders = new List<VolumeSlider>();
    private readonly List<VolumeSlider> volumeSliders = new List<VolumeSlider>();

    // =====================================
    // GAME SETTINGS
    // =====================================
    [Header("GAME SETTINGS")]
    [SerializeField] private int startingCoins = 500;
    [SerializeField] private int totalWaves = 5;

    private int currentCoins;
    private int currentWave = 1;
    private bool gameEnded = false;

    // WaveManager o'yin boshlanishini kutishi uchun
    public bool IsGameStarted { get; private set; }

    // =====================================
    // UNITY
    // =====================================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CreateEnemyBarContainer();
    }

    private void CreateEnemyBarContainer()
    {
        Transform parent = gamePanel != null ? gamePanel.transform : transform;

        GameObject go = new GameObject("EnemyHealthBars", typeof(RectTransform));

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);

        // Game UI elementlari (tugmalar, coin) HP bar ustida turadi
        rt.SetAsFirstSibling();

        EnemyBarContainer = rt;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        SetupButtons();
        InitializeSettings();

        ResetGameUI();

        if (startImmediately)
        {
            startImmediately = false;
            StartGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    // =====================================
    // BUTTON WIRING (hammasi kodda)
    // =====================================

    // Nomni solishtirish uchun tozalaydi:
    // "Start Button", "start_btn", "StartButton" -> "start"
    private static string Normalize(string value)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
                sb.Append(char.ToLowerInvariant(c));
        }

        string result = sb.ToString();

        if (result.EndsWith("button") && result.Length > 6)
            result = result.Substring(0, result.Length - 6);
        else if (result.EndsWith("btn") && result.Length > 3)
            result = result.Substring(0, result.Length - 3);

        return result;
    }

    // Panel ichidan (nofaol obyektlar ham) nomi bo'yicha Button topadi
    private Button FindButton(GameObject panel, string key)
    {
        if (panel == null)
            return null;

        foreach (Button button in panel.GetComponentsInChildren<Button>(true))
        {
            if (Normalize(button.name) == key)
                return button;
        }

        Debug.LogWarning("Button topilmadi: '" + key + "' | Panel: " + panel.name);
        return null;
    }

    // Panel ichidan nomi bo'yicha oddiy obyekt topadi (inactive tower uchun)
    private GameObject FindObject(GameObject panel, string key)
    {
        if (panel == null)
            return null;

        foreach (Transform t in panel.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject != panel && Normalize(t.name) == key)
                return t.gameObject;
        }

        Debug.LogWarning("Obyekt topilmadi: '" + key + "' | Panel: " + panel.name);
        return null;
    }

    private void Bind(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();

            action();
        });
    }

    private void SetupButtons()
    {
        // MAIN MENU: Start, Settings, Quit
        startButton = FindButton(mainMenuPanel, "start");
        settingsButton = FindButton(mainMenuPanel, "settings");
        quitButton = FindButton(mainMenuPanel, "quit");

        // GAME PANEL: Pause
        pauseButton = FindButton(gamePanel, "pause");

        // SETTINGS: Back
        settingsBackButton = FindButton(settingsPanel, "back");

        // PAUSE: Restart, MainMenu, Back
        pauseRestartButton = FindButton(pausePanel, "restart");
        pauseMainMenuButton = FindButton(pausePanel, "mainmenu");
        pauseBackButton = FindButton(pausePanel, "back");

        // YOU WIN: Replay, MainMenu, Next
        winReplayButton = FindButton(youWinPanel, "replay");
        winMainMenuButton = FindButton(youWinPanel, "mainmenu");
        winNextButton = FindButton(youWinPanel, "next");

        // YOU DIED: Retry, MainMenu
        diedRetryButton = FindButton(youDiedPanel, "retry");
        diedMainMenuButton = FindButton(youDiedPanel, "mainmenu");

        Bind(startButton, StartGame);
        Bind(settingsButton, OpenSettings);
        Bind(quitButton, QuitGame);

        Bind(pauseButton, PauseGame);

        Bind(settingsBackButton, SettingsBack);

        Bind(pauseRestartButton, RestartLevel);
        Bind(pauseMainMenuButton, GoToMainMenu);
        Bind(pauseBackButton, ResumeGame);

        Bind(winReplayButton, ReplayLevel);
        Bind(winMainMenuButton, GoToMainMenu);
        Bind(winNextButton, NextLevel);

        Bind(diedRetryButton, RetryLevel);
        Bind(diedMainMenuButton, GoToMainMenu);

        // TOWER: Tower1..Tower4 va Tower1_Inactive..Tower4_Inactive (game panel ichida)
        for (int i = 0; i < towerActiveButtons.Length; i++)
        {
            int index = i; // closure uchun

            towerActiveButtons[i] = FindButton(gamePanel, "tower" + (i + 1));
            towerInactiveButtons[i] = FindObject(gamePanel, "tower" + (i + 1) + "inactive");

            Bind(towerActiveButtons[i], () => SelectTower(index));
        }
    }

    // =====================================
    // PANELS
    // =====================================
    private void HideAllPanels()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(gamePanel, false);
        SetPanel(settingsPanel, false);
        SetPanel(pausePanel, false);
        SetPanel(youWinPanel, false);
        SetPanel(youDiedPanel, false);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    // =====================================
    // MAIN MENU
    // =====================================
    public void ShowMainMenu()
    {
        // Menyuda o'yin (wave, enemy) boshlanib ketmasligi uchun vaqt to'xtatiladi
        Time.timeScale = 0f;
        IsGameStarted = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackgroundMusic();

        HideAllPanels();
        SetPanel(mainMenuPanel, true);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        gameEnded = false;
        IsGameStarted = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackgroundMusic();

        HideAllPanels();
        SetPanel(gamePanel, true);

        ResetGameUI();
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        PlayerPrefs.Save();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // =====================================
    // SETTINGS (main menu'dan ochiladi)
    // =====================================
    public void OpenSettings()
    {
        SetPanel(mainMenuPanel, false);
        SetPanel(settingsPanel, true);
    }

    public void SettingsBack()
    {
        PlayerPrefs.Save();
        SetPanel(settingsPanel, false);
        SetPanel(mainMenuPanel, true);
    }

    // =====================================
    // PAUSE
    // =====================================
    public void PauseGame()
    {
        if (gameEnded) return;

        Time.timeScale = 0f;

        SetPanel(gamePanel, false);
        SetPanel(pausePanel, true);
    }

    public void ResumeGame()
    {
        PlayerPrefs.Save();
        Time.timeScale = 1f;

        SetPanel(pausePanel, false);
        SetPanel(gamePanel, true);
    }

    // Sahnani qayta yuklaydi va menyusiz darhol o'yinni boshlaydi
    public void RestartLevel()
    {
        startImmediately = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Sahnani qayta yuklaydi va main menuni ko'rsatadi
    // (o'yin holati - enemy, tower, wave - to'liq tozalanadi)
    public void GoToMainMenu()
    {
        startImmediately = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =====================================
    // WIN
    // =====================================
    // WaveManager: oxirgi wave tugab, enemylar qolmaganda chaqiradi
    public void ShowWinPanel()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWinMusic();

        SetPanel(gamePanel, false);
        SetPanel(youWinPanel, true);
    }

    public void ReplayLevel()
    {
        RestartLevel();
    }

    public void NextLevel()
    {
        // Keyingi sahna build settings'da bo'lsa, shunga o'tadi
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
        {
            startImmediately = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(next);
        }
        else
        {
            Debug.LogWarning("Keyingi level yo'q. Build Settings'ga yangi sahna qo'shing.");
        }
    }

    // =====================================
    // GAME OVER
    // =====================================
    // BaseHealth: HP 0 bo'lganda chaqiradi
    public void ShowDiedPanel()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOverMusic();

        SetPanel(gamePanel, false);
        SetPanel(youDiedPanel, true);
    }

    public void RetryLevel()
    {
        RestartLevel();
    }

    // =====================================
    // COINS
    // =====================================
    // Enemy o'lganda: UIManager.Instance.AddCoins(reward);
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;

        UpdateCoinUI();
        UpdateTowerButtons();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;
        if (currentCoins < amount) return false;

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
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }

    // =====================================
    // TOWER BUTTONS (active / inactive)
    // =====================================
    private void UpdateTowerButtons()
    {
        int count = Mathf.Min(
            towerPrices.Length,
            Mathf.Max(towerActiveButtons.Length, towerInactiveButtons.Length)
        );

        for (int i = 0; i < count; i++)
        {
            bool canBuy = currentCoins >= towerPrices[i];

            if (i < towerActiveButtons.Length && towerActiveButtons[i] != null)
                towerActiveButtons[i].gameObject.SetActive(canBuy);

            if (i < towerInactiveButtons.Length && towerInactiveButtons[i] != null)
                towerInactiveButtons[i].SetActive(!canBuy);
        }
    }

    public void SelectTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrices.Length)
            return;

        if (currentCoins < towerPrices[towerIndex])
            return;

        if (towerBuildManager != null)
            towerBuildManager.SelectTower(towerIndex);
        else
            Debug.LogError("TowerBuildManager ulanmagan!");
    }

    // Tower muvaffaqiyatli joylashtirilgandan keyin chaqiriladi
    public bool BuyTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrices.Length)
            return false;

        return SpendCoins(towerPrices[towerIndex]);
    }

    public int GetTowerPrice(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrices.Length)
            return 0;

        return towerPrices[towerIndex];
    }

    // =====================================
    // BASE HEALTH
    // =====================================
    public void SetBaseHealth(float currentHealth, float maxHealth)
    {
        if (baseHealthFill == null || maxHealth <= 0f)
            return;

        baseHealthFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }

    // =====================================
    // WAVE
    // =====================================
    // WaveManager: yangi wave boshlanganda UIManager.Instance.SetWave(n);
    public void SetWave(int wave)
    {
        currentWave = Mathf.Clamp(wave, 1, totalWaves);
        UpdateWaveUI();
    }

    public int GetTotalWaves()
    {
        return totalWaves;
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
            waveText.text = currentWave + " / " + totalWaves;
    }

    // =====================================
    // AUDIO (settings va pause sliderlari sinxron)
    // =====================================
    private void CollectSliders(GameObject panel)
    {
        if (panel == null)
            return;

        int musicCount = 0;
        int volumeCount = 0;

        foreach (VolumeSlider slider in panel.GetComponentsInChildren<VolumeSlider>(true))
        {
            if (slider.Type == VolumeSliderType.Music)
            {
                musicCount++;
                musicSliders.Add(slider);
                slider.OnValueChanged += SetMusicVolume;
            }
            else
            {
                volumeCount++;
                volumeSliders.Add(slider);
                slider.OnValueChanged += SetGameVolume;
            }
        }

        if (musicCount != 1 || volumeCount != 1)
        {
            Debug.LogWarning(
                panel.name + ": Music slider = " + musicCount +
                ", Volume slider = " + volumeCount +
                " (har biridan bittadan bo'lishi kerak. Type'ni tekshiring)"
            );
        }
    }

    private void InitializeSettings()
    {
        CollectSliders(settingsPanel);
        CollectSliders(pausePanel);

        float savedMusic = AudioManager.Instance != null
            ? AudioManager.Instance.MusicVolume
            : PlayerPrefs.GetFloat("MusicVolume", 1f);

        float savedVolume = AudioManager.Instance != null
            ? AudioManager.Instance.GameVolume
            : PlayerPrefs.GetFloat("GameVolume", 1f);

        foreach (VolumeSlider slider in musicSliders)
            slider.SetValueWithoutNotify(savedMusic);

        foreach (VolumeSlider slider in volumeSliders)
            slider.SetValueWithoutNotify(savedVolume);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
        else
            PlayerPrefs.SetFloat("MusicVolume", value);

        foreach (VolumeSlider slider in musicSliders)
            slider.SetValueWithoutNotify(value);
    }

    public void SetGameVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetGameVolume(value);
        else
            AudioListener.volume = value;

        foreach (VolumeSlider slider in volumeSliders)
            slider.SetValueWithoutNotify(value);
    }

    // =====================================
    // RESET
    // =====================================
    public void ResetGameUI()
    {
        currentCoins = startingCoins;
        currentWave = 1;

        UpdateCoinUI();
        UpdateWaveUI();
        UpdateTowerButtons();

        SetBaseHealth(100f, 100f);
    }
}