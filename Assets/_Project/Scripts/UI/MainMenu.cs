using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Para activar/desactivar botones visualmente

public class MainMenu : MonoBehaviour
{
    public Button continueButton; // Arrastra aquí tu botón de continuar en el Inspector

    private void Start()
    {
        // Si hay una partida guardada, activamos el botón de Continuar. Si no, lo apagamos.
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.HasSaveFile();
        }
    }

    public void StartNewGame()
    {
        SaveSystem.DeleteSave(); // Borramos cualquier partida vieja
        UIManager.currentFloor = 1; // Reiniciamos el piso
        SceneManager.LoadScene("LevelGenerationScene"); // Pon el nombre exacto de tu escena
    }

    public void ContinueGame()
    {
        if (SaveSystem.HasSaveFile())
        {
            SaveData data = SaveSystem.Load();
            UIManager.currentFloor = data.floor; // Recuperamos el piso donde nos quedamos
            SceneManager.LoadScene("LevelGenerationScene"); // Pon el nombre exacto de tu escena
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}