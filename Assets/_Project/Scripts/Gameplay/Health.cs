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
    public bool isBoss = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

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

        if (SoundManager.instance != null)
        {
            if (isPlayer) SoundManager.instance.PlaySFX(SoundManager.instance.playerHurtClip);
            else SoundManager.instance.PlaySFX(SoundManager.instance.enemyHurtClip);
        }

        if (isPlayer)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
            }

            if (CameraFollow.instance != null)
            {
                CameraFollow.instance.TriggerShake(0.15f, 0.1f);
            }
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

    // --- NUEVO: SISTEMA DE EMPUJÓN (KNOCKBACK) ---
    public void ApplyKnockback(Vector3 attackerPosition, float force, float duration = 0.15f)
    {
        // Los jefes son demasiado grandes para ser empujados, y los muertos no se mueven
        if (isBoss || currentHealth <= 0) return;

        StartCoroutine(KnockbackRoutine(attackerPosition, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector3 attackerPosition, float force, float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 1. Apagamos los scripts de movimiento para que no luchen contra la física
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            ArcherAI archerAI = GetComponent<ArcherAI>();
            PlayerController playerCtrl = GetComponent<PlayerController>();

            if (enemyAI != null) enemyAI.enabled = false;
            if (archerAI != null) archerAI.enabled = false;
            if (playerCtrl != null) playerCtrl.enabled = false;

            // 2. Calculamos la dirección contraria a ti y aplicamos la fuerza
            Vector2 direction = (transform.position - attackerPosition).normalized;
            rb.linearVelocity = direction * force;

            // 3. Esperamos el tiempo que dura el empujón
            yield return new WaitForSeconds(duration);

            // 4. Frenamos en seco
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 5. Devolvemos el control al enemigo si sigue vivo
            if (currentHealth > 0)
            {
                if (enemyAI != null) enemyAI.enabled = true;
                if (archerAI != null) archerAI.enabled = true;
                if (playerCtrl != null) playerCtrl.enabled = true;
            }
        }
    }
    // ----------------------------------------------

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
                LootDropper lootDropper = GetComponent<LootDropper>();
                if (lootDropper != null) lootDropper.TryDropLoot();

                Animator anim = GetComponentInChildren<Animator>();
                if (anim == null) anim = GetComponent<Animator>();
                if (anim != null) anim.SetTrigger("Die");

                EnemyAI ai = GetComponent<EnemyAI>();
                if (ai != null) ai.enabled = false;

                ArcherAI archerAi = GetComponent<ArcherAI>();
                if (archerAi != null) archerAi.enabled = false;

                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                Destroy(gameObject, 1f);
            }
        }
    }

    private IEnumerator BossFlashRoutine(Color flashColor, float duration)
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator BossDeathRoutine()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) script.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

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

        LootDropper lootDropper = GetComponent<LootDropper>();
        if (lootDropper != null) lootDropper.TryDropLoot();

        Destroy(gameObject);
    }
}