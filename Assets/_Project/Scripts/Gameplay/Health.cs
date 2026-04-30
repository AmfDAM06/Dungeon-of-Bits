using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Estadísticas")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invulnerabilidad")]
    public bool isPlayer = false; // Marcamos esto solo en el jugador
    public float iFramesDuration = 1.5f; // Duración de la inmunidad
    public float blinkInterval = 0.1f; // Velocidad del parpadeo

    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        // Buscamos el SpriteRenderer (ya sea en este objeto o en sus hijos)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        // Si somos invulnerables, ignoramos el código de abajo y no recibimos daño
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " ha recibido daño. Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (isPlayer)
        {
            // Solo activamos los i-frames si este script pertenece al jugador
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsedTime = 0f;

        // Bucle que dura el tiempo de invulnerabilidad
        while (elapsedTime < iFramesDuration)
        {
            if (spriteRenderer != null)
            {
                // Alterna entre activado y desactivado para crear el parpadeo
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Al terminar, nos aseguramos de que el sprite se quede visible
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha sido destruido.");
        Destroy(gameObject);
    }
}