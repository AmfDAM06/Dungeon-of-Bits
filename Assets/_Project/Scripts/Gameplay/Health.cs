using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invulnerability")]
    public bool isPlayer = false;
    public float iFramesDuration = 1.5f;
    public float blinkInterval = 0.1f;

    private bool isInvulnerable = false;
    public bool isShielded = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isShielded) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage. HP: " + currentHealth);

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (isPlayer)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    // --- NUEVO: MÉTODO PARA CURAR ---
    public bool Heal(int amount)
    {
        // Si ya tenemos la vida al máximo, devolvemos 'false' para no gastar la poción
        if (currentHealth >= maxHealth)
        {
            return false;
        }

        currentHealth += amount;

        // Nos aseguramos de no superar la vida máxima
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(gameObject.name + " healed. HP: " + currentHealth);

        // Actualizamos la UI
        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        return true; // Devolvemos 'true' indicando que la curación fue un éxito
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsedTime = 0f;

        while (elapsedTime < iFramesDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        isInvulnerable = false;
    }

    // He añadido la lógica del loot aquí
    private void Die()
    {
        Debug.Log(gameObject.name + " was destroyed.");

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
        else // Si NO es el jugador (es un enemigo u otro objeto destruible)
        {
            // NUEVO: Intentamos soltar loot antes de morir
            LootDropper lootDropper = GetComponent<LootDropper>();
            if (lootDropper != null)
            {
                lootDropper.TryDropLoot();
            }
        }

        Destroy(gameObject);
    }
}