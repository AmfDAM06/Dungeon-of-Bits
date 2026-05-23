using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;
    public string targetTag = "Enemy";
    public bool isBuffed = false; // <--- NUEVO

    [Header("Configuración de Ataque")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;
    private float lastRailHitTime = -999f;

    [Header("Juice (Sensación de Impacto)")]
    public bool useHitStop = true;
    public float hitStopDuration = 0.05f;
    public float knockbackForce = 10f; // <--- NUEVO: La fuerza del empujón

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision) { TryDealDamage(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { TryDealDamage(collision.gameObject); }
    private void OnTriggerStay2D(Collider2D collision) { TryDealDamage(collision.gameObject); }
    private void OnCollisionStay2D(Collision2D collision) { TryDealDamage(collision.gameObject); }

    private void TryDealDamage(GameObject targetObj)
    {
        PuzzleSwitch puzzleSwitch = targetObj.GetComponent<PuzzleSwitch>();
        if (puzzleSwitch != null)
        {
            puzzleSwitch.ToggleByHit();
            return;
        }

        PuzzleRail puzzleRail = targetObj.GetComponent<PuzzleRail>();
        if (puzzleRail != null)
        {
            if (Time.time >= lastRailHitTime + 0.2f)
            {
                puzzleRail.RotateRail();
                lastRailHitTime = Time.time;
            }
            return;
        }

        Minecart minecart = targetObj.GetComponent<Minecart>();
        if (minecart != null)
        {
            minecart.ActivateCart();
            return;
        }

        if (targetObj.CompareTag(targetTag))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Health targetHealth = targetObj.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damageAmount);

                    // --- NUEVO: APLICAMOS EL EMPUJÓN HACIA ATRÁS ---
                    targetHealth.ApplyKnockback(transform.position, knockbackForce);

                    lastAttackTime = Time.time;

                    if (SoundManager.instance != null)
                    {
                        SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
                    }

                    if (spriteRenderer != null && gameObject.activeInHierarchy)
                    {
                        StartCoroutine(AttackVisualFeedback());
                    }

                    if (useHitStop)
                    {
                        StartCoroutine(HitStopRoutine());
                    }
                }
            }
        }
    }

    private IEnumerator AttackVisualFeedback()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    private IEnumerator HitStopRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }
}