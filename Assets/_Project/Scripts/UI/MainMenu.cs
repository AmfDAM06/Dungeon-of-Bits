using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles de UI")]
    public Button continueButton;
    public GameObject mainButtonsPanel;
    public GameObject instructionsPanel;

    [Header("Configuración de Escenas")]
    [Tooltip("Escribe aquí el nombre EXACTO de la escena de tu mazmorra")]
    public string dungeonSceneName = "LevelGenerationScene";

    private void Start()
    {
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

    // --- NUEVO: Detectar la tecla ESCAPE en todo momento ---
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Si el panel de instrucciones está activado/visible, lo cerramos
            if (instructionsPanel != null && instructionsPanel.activeSelf)
            {
                CloseInstructions();
            }
        }
    }
    // -------------------------------------------------------

    public void OpenInstructions()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

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