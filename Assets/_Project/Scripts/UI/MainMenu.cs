using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MainMenu : MonoBehaviour
{
    // Método que llamará el botón START GAME
    public void StartGame()
    {
        // Cargamos la escena del juego (asegúrate de que el nombre coincide exactamente)
        // También puedes usar SceneManager.LoadScene(1); si la pusiste en el índice 1
        SceneManager.LoadScene("LevelGenerationScene");
    }

    // Método que llamará el botón QUIT
    public void QuitGame()
    {
        // Esto cierra el juego cuando está compilado (.exe)
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}