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

    [Header("Efectos de Jefe (Sin animaciones)")]
    public bool isBoss = false; // <-- Asegúrate de marcar esto solo en el Inspector del Ninja

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Buscamos el SpriteRenderer (en este objeto o en los hijos)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // SISTEMA DE CARGA DE VIDA
        if (isPlayer)
        {
            SaveData data = SaveSystem.Load();
            if (data != null)
            {
                maxHealth = data.maxHealth;
                currentHealth = data.currentHealth;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || isShielded) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " ha recibido daño. Vida restante: " + currentHealth);

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (isPlayer)
            {
                StartCoroutine(InvulnerabilityRoutine());
            }
            else
            {
                // --- LÓGICA DE DAÑO: JEFE O ENEMIGO NORMAL ---
                if (isBoss)
                {
                    StartCoroutine(BossFlashRoutine(Color.red, 0.15f));
                }
                else
                {
                    Animator anim = GetComponentInChildren<Animator>();
                    if (anim == null) anim = GetComponent<Animator>();
                    if (anim != null) anim.SetTrigger("Hurt");
                }
            }
        }
    }

    public bool Heal(int amount)
    {
        if (currentHealth >= maxHealth) return false;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (isPlayer && UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }

        return true;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        float elapsedTime = 0f;

        while (elapsedTime < iFramesDuration)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " was destroyed.");

        if (isPlayer)
        {
            if (UIManager.Instance != null)
            {
                SaveSystem.DeleteSave();
                UIManager.Instance.ShowGameOver();
            }
            Destroy(gameObject);
        }
        else
        {
            if (isBoss)
            {
                StartCoroutine(BossDeathRoutine());
            }
            else
            {
                // --- MUERTE DE ENEMIGO NORMAL ---
                LootDropper lootDropper = GetComponent<LootDropper>();
                if (lootDropper != null) lootDropper.TryDropLoot();

                Animator anim = GetComponentInChildren<Animator>();
                if (anim == null) anim = GetComponent<Animator>();
                if (anim != null) anim.SetTrigger("Die");

                EnemyAI ai = GetComponent<EnemyAI>();
                if (ai != null) ai.enabled = false;

                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                Destroy(gameObject, 1f); // Se destruye en 1 segundo para ver la animación
            }
        }
    }

    // ==========================================
    // NUEVAS RUTINAS EXCLUSIVAS PARA EL JEFE
    // ==========================================

    private IEnumerator BossFlashRoutine(Color flashColor, float duration)
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white; // Reseteamos al color base
        }
    }

    private IEnumerator BossDeathRoutine()
    {
        // 1. Apagamos todos los scripts (excepto este) para que deje de moverse y atacar
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) script.enabled = false;
        }

        // 2. Apagamos su colisionador
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 3. Efecto visual: Parpadea en blanco y negro súper rápido (Bomba de humo)
        if (spriteRenderer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.black;
                yield return new WaitForSeconds(0.1f);
            }
        }

        // 4. Tira el botín especial (Llave, cofre, etc)
        LootDropper lootDropper = GetComponent<LootDropper>();
        if (lootDropper != null) lootDropper.TryDropLoot();

        // 5. Destruimos al jefe de la escena
        Destroy(gameObject);
    }
}