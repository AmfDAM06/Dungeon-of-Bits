using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    public Button continueButton;
    public GameObject mainButtonsPanel; // <-- NUEVO: El panel con New Game, Continue, Quit...
    public GameObject instructionsPanel; // <-- NUEVO: Tu nuevo panel de instrucciones

    [Header("Configuración de Escenas")]
    [Tooltip("Escribe aquí el nombre EXACTO de la escena de tu mazmorra")]
    public string dungeonSceneName = "LevelGenerationScene";

    private void Start()
    {
        // Nos aseguramos de que el estado inicial sea el correcto
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.mainMenuMusic);
        }

        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSaveFile();
        }
    }

    // --- NUEVAS FUNCIONES PARA EL PANEL DE INSTRUCCIONES ---
    public void OpenInstructions()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false); // Ocultamos los botones principales
        if (instructionsPanel != null) instructionsPanel.SetActive(true);  // Mostramos las instrucciones
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false); // Ocultamos instrucciones
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);   // Volvemos a mostrar los botones
    }
    // -------------------------------------------------------

    public void StartNewGame()
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayMusic(SoundManager.instance.bgMusic);

        SaveSystem.DeleteSave();
        UIManager.currentFloor = 1;
        PlayerInventory.ResetInventory();

        SceneManager.LoadScene(dungeonSceneName);
    }

    public void ContinueGame()
    {
        if (SoundManager.instance != null) SoundManager.instance.PlayMusic(SoundManager.instance.bgMusic);

        if (SaveSystem.HasSaveFile())
        {
            SaveData data = SaveSystem.Load();
            UIManager.currentFloor = data.floor;

            PlayerInventory.globalHealthPotions = data.healthPotions;
            PlayerInventory.globalInvisPotions = data.invisPotions;
            PlayerInventory.globalStrengthPotions = data.strengthPotions;
            PlayerInventory.globalBombs = data.bombs;

            SceneManager.LoadScene(dungeonSceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}