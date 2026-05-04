using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Necesario para TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Dungeon Progress")]
    public static int currentFloor = 1;
    public TextMeshProUGUI floorText;

    [Header("Health UI")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Actualizado al inglés
        if (floorText != null)
        {
            floorText.text = "Floor: " + currentFloor;
        }
    }

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        currentFloor = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}