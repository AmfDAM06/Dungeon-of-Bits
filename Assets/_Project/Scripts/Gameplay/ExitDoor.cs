using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Subimos de piso
            UIManager.currentFloor++;

            // 2. Guardamos la partida con la vida exacta que tiene el jugador
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                SaveSystem.Save(UIManager.currentFloor, playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
            }

            // 3. Generamos la nueva mazmorra
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}