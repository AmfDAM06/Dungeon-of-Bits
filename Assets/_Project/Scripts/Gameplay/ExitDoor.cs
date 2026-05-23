using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- NUEVO: Sonido de Fanfarria / Victoria ---
            if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.levelCompleteClip);

            UIManager.currentFloor++;

            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                SaveSystem.Save(UIManager.currentFloor, playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}