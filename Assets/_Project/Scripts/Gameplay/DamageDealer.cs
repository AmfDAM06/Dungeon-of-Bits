using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;
    public string targetTag = "Enemy";

    [Header("Configuración de Ataque")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

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

        if (targetObj.CompareTag(targetTag))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Health targetHealth = targetObj.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damageAmount);
                    lastAttackTime = Time.time;

                    // Activamos el color rojo de golpe
                    if (spriteRenderer != null && gameObject.activeInHierarchy)
                    {
                        StartCoroutine(AttackVisualFeedback());
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
}