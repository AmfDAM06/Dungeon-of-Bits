using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Configuración de Escenas")]
    public string mainMenuSceneName = "MainMenuScene";

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Inventory UI")]
    public TextMeshProUGUI healthPotionText;
    public TextMeshProUGUI invisPotionText;
    public TextMeshProUGUI strengthPotionText;
    public TextMeshProUGUI bombText;

    [Header("Gameplay UI")]
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI puzzleText;

    public static int currentFloor = 1;
    private bool isPaused = false;

    void Awake()
    {
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

    void Start()
    {
        ShowGameplay();
        UpdateFloorUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverPanel.activeSelf && !HackingTerminal.isTerminalOpen)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            if (pausePanel != null) pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeartSprite;
                heartImages[i].enabled = true;
            }
            else if (i < maxHealth)
            {
                heartImages[i].sprite = emptyHeartSprite;
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }
    }

    public void UpdateInventoryUI(int health, int invis, int strength, int bombs)
    {
        if (healthPotionText != null) healthPotionText.text = "x" + health;
        if (invisPotionText != null) invisPotionText.text = "x" + invis;
        if (strengthPotionText != null) strengthPotionText.text = "x" + strength;
        if (bombText != null) bombText.text = "x" + bombs;
    }

    public void UpdateFloorUI()
    {
        if (floorText != null) floorText.text = "Floor: " + currentFloor;
    }

    public void UpdatePuzzleUI(int activated, int total)
    {
        if (puzzleText != null)
        {
            if (activated >= total) puzzleText.text = "<color=green>Door Open!</color>";
            else puzzleText.text = $"Puzzles: {activated} / {total}";
        }
    }

    public void ShowGameplay()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        PlayerInventory.ResetInventory();
        currentFloor = 1;
        UpdateFloorUI();
        ShowGameplay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // --- NUEVO: Guardar la partida antes de volver al menú ---
        SaveCurrentGame();

        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- NUEVO: Guardar al pulsar la X de la ventana o ALT+F4 ---
    private void OnApplicationQuit()
    {
        SaveCurrentGame();
    }

    // --- NUEVO: Lógica unificada para guardar ---
    private void SaveCurrentGame()
    {
        // Buscamos al jugador en la escena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();

            // Solo guardamos si el jugador existe y NO está muerto (para mantener el Game Over permadeath)
            if (playerHealth != null && playerHealth.GetCurrentHealth() > 0)
            {
                SaveSystem.Save(currentFloor, playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
            }
        }
    }
}