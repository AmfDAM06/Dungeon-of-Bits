using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Sumamos 1 al contador estático de nuestro UIManager
            UIManager.currentFloor++;

            // Recargamos la escena para generar el siguiente piso
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}