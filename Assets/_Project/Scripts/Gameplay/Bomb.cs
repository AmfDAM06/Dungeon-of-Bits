using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Configuración de la Explosión")]
    public float delay = 2f;
    public float explosionRadius = 1.5f;
    public int damage = 3;
    public float knockbackForce = 12f;

    [Header("Efecto Visual")]
    public GameObject explosionEffectPrefab;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        float timePassed = 0f;
        float blinkSpeed = 0.2f;

        while (timePassed < delay)
        {
            if (sr != null) sr.color = Color.red;
            yield return new WaitForSeconds(blinkSpeed);

            if (sr != null) sr.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);

            timePassed += blinkSpeed * 2;
            blinkSpeed = Mathf.Max(0.05f, blinkSpeed - 0.02f);
        }

        Explode();
    }

    private void Explode()
    {
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (sr != null) sr.enabled = false;

        yield return new WaitForSeconds(0.8f);

        // --- NUEVO: Sonido de la explosión ---
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySFX(SoundManager.instance.bombExplosionClip);

        if (CameraFollow.instance != null)
        {
            CameraFollow.instance.TriggerShake(0.3f, 0.6f);
        }

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy") || obj.CompareTag("Player"))
            {
                Health health = obj.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    health.ApplyKnockback(transform.position, knockbackForce, 0.2f);
                }
            }

            PuzzleSwitch pSwitch = obj.GetComponent<PuzzleSwitch>();
            if (pSwitch != null && pSwitch.type == PuzzleSwitch.SwitchType.Melee)
            {
                pSwitch.ToggleByHit();
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}