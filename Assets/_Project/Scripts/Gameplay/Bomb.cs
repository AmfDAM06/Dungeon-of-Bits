using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    [Header("Configuración de la Explosión")]
    public float delay = 2f;             // Tiempo hasta explotar
    public float explosionRadius = 1.5f; // Área de la explosión
    public int damage = 3;               // Daño que hace
    public float knockbackForce = 12f;   // Fuerza con la que empuja

    [Header("Efecto Visual")]
    public GameObject explosionEffectPrefab; // El sprite/partículas de explosión

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

        // La bomba parpadea en rojo cada vez más rápido
        while (timePassed < delay)
        {
            if (sr != null) sr.color = Color.red;
            yield return new WaitForSeconds(blinkSpeed);

            if (sr != null) sr.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);

            timePassed += blinkSpeed * 2;
            blinkSpeed = Mathf.Max(0.05f, blinkSpeed - 0.02f); // Acelera el parpadeo
        }

        Explode();
    }

    private void Explode()
    {
        // En lugar de explotar de golpe, iniciamos una secuencia
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        // 1. Mostrar la animación visual de la explosión
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        // 2. Escondemos el dibujo de la bomba en el suelo para que no se superponga con la explosión
        if (sr != null) sr.enabled = false;

        // 3. LA CLAVE: Esperamos una fracción de segundo para que la animación llegue a su punto "fuerte".
        // (Puedes subir o bajar este 0.1f hasta que sincronice perfecto con tu dibujo).
        yield return new WaitForSeconds(0.8f);

        // 4. Ahora sí: Temblor de cámara épico
        if (CameraFollow.instance != null)
        {
            CameraFollow.instance.TriggerShake(0.3f, 0.6f);
        }

        // 5. Aplicar daño y empuje a los enemigos cercanos
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

        // 6. Finalmente, destruimos el objeto lógico de la bomba
        Destroy(gameObject);
    }

    // Esto dibuja un círculo rojo en la escena de Unity para que veas el radio de la explosión
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}