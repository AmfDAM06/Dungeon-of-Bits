using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Usamos TMPro para los textos

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

    [Header("Gameplay UI")]
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public TextMeshProUGUI floorText;

    // --- NUEVO: Casilla para arrastrar el texto del contador de puzles ---
    public TextMeshProUGUI puzzleText;

    public static int currentFloor = 1;

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

    public void UpdateFloorUI()
    {
        if (floorText != null)
        {
            floorText.text = "Floor: " + currentFloor;
        }
    }

    // --- NUEVA FUNCIÓN: Actualiza el texto de los puzles en pantalla ---
    public void UpdatePuzzleUI(int activated, int total)
    {
        if (puzzleText != null)
        {
            if (activated >= total)
            {
                puzzleText.text = "<color=green>¡Puerta Abierta!</color>";
            }
            else
            {
                puzzleText.text = $"Puzles: {activated} / {total}";
            }
        }
    }

    public void ShowGameplay()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        currentFloor = 1;
        UpdateFloorUI();
        ShowGameplay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        currentFloor = 1;
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}