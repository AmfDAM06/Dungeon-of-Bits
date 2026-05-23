using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button continueButton;

    [Header("Configuración de Escenas")]
    [Tooltip("Escribe aquí el nombre EXACTO de la escena de tu mazmorra")]
    public string dungeonSceneName = "LevelGenerationScene";

    private void Start()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.mainMenuMusic);
        }

        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSaveFile();
        }
    }

    public void StartNewGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.bgMusic);
        }

        SaveSystem.DeleteSave();
        UIManager.currentFloor = 1;
        PlayerInventory.ResetInventory(); // Vaciamos los bolsillos

        SceneManager.LoadScene(dungeonSceneName);
    }

    public void ContinueGame()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayMusic(SoundManager.instance.bgMusic);
        }

        if (SaveSystem.HasSaveFile())
        {
            SaveData data = SaveSystem.Load();
            UIManager.currentFloor = data.floor;

            // --- NUEVO: Recuperamos la mochila ---
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