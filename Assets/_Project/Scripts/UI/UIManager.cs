using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel; // <-- NUEVO: Ranura para tu panel de pausa

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
    private bool isPaused = false; // <-- NUEVO: Para saber si estamos en pausa

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
        // --- NUEVO: Lógica para pausar con Escape ---
        // Solo pausamos si NO estamos muertos y la Terminal de Hackeo NO está abierta
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverPanel.activeSelf && !HackingTerminal.isTerminalOpen)
        {
            TogglePause();
        }
    }

    // --- NUEVAS FUNCIONES DE PAUSA ---
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            if (pausePanel != null) pausePanel.SetActive(true);
            Time.timeScale = 0f; // Congela el juego
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
        Time.timeScale = 1f; // Descongela el juego
    }

    // ---------------------------------

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
        if (pausePanel != null) pausePanel.SetActive(false); // Nos aseguramos de ocultar la pausa al empezar
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false); // Oculta la pausa si mueres
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
        PlayerInventory.ResetInventory();
        currentFloor = 1;
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}