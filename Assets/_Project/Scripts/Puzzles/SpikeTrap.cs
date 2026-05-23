using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Daño y Empuje")]
    public int damage = 1;
    public float knockbackForce = 8f;

    [Header("Tiempos (Ritmo)")]
    public float hiddenTime = 2f;  // Tiempo bajo tierra
    public float warningTime = 0.5f; // Tiempo que tiembla/asoma antes de salir
    public float activeTime = 1.5f;  // Tiempo que se queda fuera haciendo daño

    [Header("Sprites (Tus 4 dibujos)")]
    public Sprite hiddenSprite;  // Suelo normal o agujeros vacíos
    public Sprite warningSprite; // Pinchos asomando un poco
    public Sprite activeSprite;  // Pinchos fuera del todo

    private SpriteRenderer sr;
    private BoxCollider2D trapCollider;
    private bool isLethal = false; // Solo hace daño cuando está activa

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<BoxCollider2D>();

        // Empezamos la rutina infinita
        StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        // Desfasamos un poco el inicio para que, si hay 10 trampas juntas, no salgan todas a la vez
        yield return new WaitForSeconds(Random.Range(0f, 1f));

        while (true)
        {
            // 1. FASE ESCONDIDA (Seguro)
            isLethal = false;
            if (sr != null && hiddenSprite != null) sr.sprite = hiddenSprite;
            yield return new WaitForSeconds(hiddenTime);

            // 2. FASE DE AVISO (Peligro inminente)
            if (sr != null && warningSprite != null) sr.sprite = warningSprite;
            yield return new WaitForSeconds(warningTime);

            // 3. FASE ACTIVA (¡Pinchos fuera!)
            isLethal = true;
            if (sr != null && activeSprite != null) sr.sprite = activeSprite;

            // Sonido de cuchillas
            if (SoundManager.instance != null && SoundManager.instance.swordSwingClip != null)
            {
                SoundManager.instance.PlaySFX(SoundManager.instance.swordSwingClip);
            }

            yield return new WaitForSeconds(activeTime);
        }
    }

    // Esta función de Unity se ejecuta continuamente mientras alguien esté pisando el collider
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Si la trampa está escondida o en fase de aviso, no hace nada
        if (!isLethal) return;

        // Si está activa y toca al jugador o a un enemigo, hace daño
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            Health health = collision.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                health.ApplyKnockback(transform.position, knockbackForce, 0.2f);
            }
        }
    }
}